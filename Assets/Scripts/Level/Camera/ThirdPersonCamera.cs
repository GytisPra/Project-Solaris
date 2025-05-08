using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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
    public float pitchSensitivity = 10f;
    public float rotateSensitivity = 80f;
    public float maxPitch = 80f;
    public float minPitch = 10f;
    public float rotateSmoothTime = 0.1f;
    public float pitchSmoothTime = 0.1f;

    // Internal state
    private InputAction scrollAction;
    private InputAction middleButton;
    private bool rotatingKBM;
    private bool isTouchGestureActive;

    private float targetYaw;
    private float currentYaw;
    private float yawVelocity;

    private float targetPitch;
    private float currentPitch;
    private float pitchVelocity;

    private float targetDistance;
    private float currentDistance;
    private float distanceVelocity;

    private void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void Start()
    {
        EnhancedTouchSupport.Enable();

        // Initialize distance based on offset magnitude
        Vector3 offset = transform.position - target.position;
        currentDistance = targetDistance = offset.magnitude;

        // Compute initial yaw and pitch
        Vector3 direction = offset.normalized;
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z).normalized;
        currentPitch = targetPitch = Vector3.SignedAngle(flatDir, direction, Vector3.Cross(flatDir, Vector3.up));
        currentYaw = targetYaw = transform.eulerAngles.y;

        // Subscribe touch gestures
        TouchGestureManager.Instance.OnPinch += HandlePinch;
        TouchGestureManager.Instance.OnRotate += HandleRotate;
        TouchGestureManager.Instance.OnDrag += HandleDrag;
        TouchGestureManager.Instance.OnGestureActive += active => isTouchGestureActive = active;

        SetupInputActions();
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
        DisposeInputActions();

        if (TouchGestureManager.Instance != null)
        {
            TouchGestureManager.Instance.OnPinch -= HandlePinch;
            TouchGestureManager.Instance.OnRotate -= HandleRotate;
            TouchGestureManager.Instance.OnDrag -= HandleDrag;
            TouchGestureManager.Instance.OnGestureActive -= active => isTouchGestureActive = active;
        }
    }

    private void Update()
    {
        HandleMouseRotation();
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Smooth yaw, pitch, and distance
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVelocity, rotateSmoothTime);
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, pitchSmoothTime);
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, zoomSmoothTime);

        // Calculate orbit position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 newPos = target.position + rotation * Vector3.back * currentDistance;

        bool userControlling = rotatingKBM || isTouchGestureActive;
        if (userControlling)
        {
            // direct snap when the user is dragging/pinching/rotating
            transform.position = newPos;
        }
        else
        {
            // blend in to maintain smooth follow if target moves
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * Time.deltaTime);
        }

        transform.LookAt(target);
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
        enabled = newState == GameState.Gameplay;
        if (enabled)
        {
            scrollAction?.Enable();
            middleButton?.Enable();
        }
        else
        {
            scrollAction?.Disable();
            middleButton?.Disable();
        }
    }

    private void HandleMouseRotation()
    {
        if (!rotatingKBM || target == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        float yawDelta = delta.x * rotateSensitivity * 0.5f * Time.deltaTime;
        float pitchDelta = -delta.y * rotateSensitivity * 0.5f * Time.deltaTime;

        targetYaw += yawDelta;
        targetPitch += pitchDelta;
    }

    private void HandlePinch(float delta)
    {
        float pinchAmount;
        // Adjust target distance (invert as needed)
        if (isTouchGestureActive)
        {
            pinchAmount = delta * zoomSpeedTouch * Time.deltaTime;
        }
        else
        {
            pinchAmount = delta * zoomSpeedKBM * Time.deltaTime;
        }
        
        targetDistance = Mathf.Clamp(
            targetDistance - pinchAmount,
            minZoomDistance,
            maxZoomDistance
        );
    }

    private void HandleRotate(float angleDelta)
    {
        targetYaw += angleDelta * rotateSensitivity * Time.deltaTime;
    }

    private void HandleDrag(Vector2 delta)
    {
        targetPitch += -delta.y * pitchSensitivity * Time.deltaTime;
    }
}
