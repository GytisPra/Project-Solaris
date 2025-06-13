using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform target;
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom")]
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 20f;
    public float zoomSpeedKBM = 2f;
    public float zoomSpeedTouch = 2f;
    public float zoomSmoothTime = 0.1f;

    [Header("Rotation")]
    public float pitchSensitivity = 50f;
    public float rotateSensitivity = 50f;
    public float maxPitch = 80f;
    public float minPitch = 10f;

    [Header("Screen Orientation change settings")]
    public float fovChangeSpeed = 5f;
    public float targetLandscapeFov = 30f;
    public float targetPortraitFov = 50f;
    public float maxZoomLandscape = 12f;
    public float maxZoomPotrait = 15f;

    // Internal state
    private InputAction scrollAction;
    private InputAction middleButton;
    private bool rotatingKBM;
    private bool userControlEnabled = true;

    private float targetYaw;
    private float targetPitch;

    private float targetDistance;
    private float currentDistance;
    private float distanceVelocity;

    private Transform thisTransform;
    private Camera thisCamera;
    [SerializeField] private JoystickTouchDetector joystickTouchDetector;

    private void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void Start()
    {
        thisTransform = transform;
        thisCamera = thisTransform.GetComponent<Camera>();

        SetupCamera();

        EnhancedTouchSupport.Enable();

        TouchGestureManager.Instance.OnPinch += HandlePinch;
        SetupInputActions();
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
        DisposeInputActions();

        if (TouchGestureManager.Instance != null)
        {
            TouchGestureManager.Instance.OnPinch -= HandlePinch;
        }
    }

    private void Update()
    {
        if (!userControlEnabled) return;

        if (joystickTouchDetector == null || !joystickTouchDetector.IsTouched)
        {
            HandleMouseRotation();
            HandleSingleFingerTouchRotation();
        }

        float targetFOV;
        switch (Screen.orientation)
        {
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                targetFOV = targetLandscapeFov;
                maxZoomDistance = maxZoomLandscape;
                break;
            case ScreenOrientation.Portrait:
                targetFOV = targetPortraitFov;
                maxZoomDistance = maxZoomPotrait;
                break;
            default:
                targetFOV = thisCamera.fieldOfView;
                break;
        }

        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, targetFOV, Time.deltaTime * fovChangeSpeed);
        thisCamera.GetComponentInChildren<Camera>().fieldOfView = targetFOV;
        targetDistance = Mathf.Clamp(targetDistance, minZoomDistance, maxZoomDistance);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }

    private void FixedUpdate()
    {
        if (target == null || !userControlEnabled) return;

        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, zoomSmoothTime);

        Quaternion rotation = Quaternion.Euler(targetPitch, targetYaw, 0);
        Vector3 newPos = target.position + rotation * Vector3.back * currentDistance;

        bool userControlling = rotatingKBM || Touch.activeTouches.Count > 0;
        if (userControlling)
        {
            thisTransform.position = newPos;
        }
        else
        {
            thisTransform.position = Vector3.Lerp(thisTransform.position, newPos, followSpeed * Time.deltaTime);
        }

        thisTransform.LookAt(target);
    }

    public IEnumerator LookAtFinish(GameObject finishTarget, float rotateDuration = 0.5f, float holdTime = 2f)
    {
        GameStateManager.Instance.SetState(GameState.Cutscene);
        bool originalUserControl = userControlEnabled;
        userControlEnabled = false;

        Vector3 direction = finishTarget.transform.position - transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        float elapsedTime = 0f;
        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotateDuration);
            yield return null;
        }

        transform.rotation = targetRotation;

        yield return new WaitForSeconds(holdTime);

        elapsedTime = 0f;
        Quaternion returnRotation = Quaternion.LookRotation(target.position - transform.position);
        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(targetRotation, returnRotation, elapsedTime / rotateDuration);
            yield return null;
        }

        userControlEnabled = originalUserControl;
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void SetupInputActions()
    {
        scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => HandlePinch(ctx.ReadValue<Vector2>().y);

        middleButton = new InputAction(binding: "<Mouse>/middleButton");
        middleButton.Enable();
        middleButton.started += _ => rotatingKBM = true;
        middleButton.canceled += _ => rotatingKBM = false;
    }

    private void DisposeInputActions()
    {
        scrollAction.Dispose();
        middleButton.Dispose();
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            scrollAction?.Enable();
            middleButton?.Enable();
            userControlEnabled = true;
        }
        else
        {
            scrollAction?.Disable();
            middleButton?.Disable();
            userControlEnabled = false;
        }
    }

    private void HandleMouseRotation()
    {
        if (!rotatingKBM || target == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        float yawDelta = delta.x * rotateSensitivity * Time.deltaTime;
        float pitchDelta = -delta.y * pitchSensitivity * Time.deltaTime;

        targetYaw += yawDelta;
        targetPitch += pitchDelta;
    }

    private void HandleSingleFingerTouchRotation()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Touch.activeTouches.Count == 1)
        {
            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta;
                float yawDelta = delta.x * rotateSensitivity * 0.35f * Time.deltaTime;
                float pitchDelta = -delta.y * pitchSensitivity * 0.35f * Time.deltaTime;

                targetYaw += yawDelta;
                targetPitch += pitchDelta;
            }
        }
#endif
    }

    private void HandlePinch(float delta)
    {
        if (joystickTouchDetector != null && joystickTouchDetector.IsTouched)
            return;

        float pinchAmount = delta * (Touch.activeTouches.Count > 1 ? zoomSpeedTouch : zoomSpeedKBM) * Time.deltaTime;
        targetDistance = Mathf.Clamp(targetDistance - pinchAmount, minZoomDistance, maxZoomDistance);
    }

    private void SetupCamera()
    {
        Vector3 offset = thisTransform.position - target.position;
        targetDistance = currentDistance = offset.magnitude;

        Vector3 direction = offset.normalized;
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z).normalized;
        targetPitch = Vector3.SignedAngle(flatDir, direction, Vector3.Cross(flatDir, Vector3.up));
        targetYaw = thisTransform.eulerAngles.y;

        Quaternion rotation = Quaternion.Euler(targetPitch, targetYaw, 0);
        Vector3 newPos = target.position + rotation * Vector3.back * currentDistance;

        thisTransform.position = newPos;
    }
}
