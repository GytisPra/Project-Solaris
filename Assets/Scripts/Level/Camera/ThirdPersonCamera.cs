using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform target;
    [SerializeField] private Vector3 offset = new(0, 5, -10);
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom")]
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 20f;
    public float zoomSpeed = 2f;

    [Header("Rotation")]
    public float pitchSensitivity = 10f;
    public float rotateSensitivity = 80f;
    public float maxPitch = 80f;
    public float minPitch = 10f;

    // Input state tracking
    private bool rotatingKBM;

    // Camera & input
    private Camera cam;

    private InputAction scrollAction;
    private InputAction middleButton;

    private void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void Start()
    {
        cam = Camera.main;
        EnhancedTouchSupport.Enable();

        TouchGestureManager.Instance.OnPinch += HandlePinch;
        TouchGestureManager.Instance.OnRotate += HandleRotate;
        TouchGestureManager.Instance.OnDrag += HandleDrag;

        SetupInputActions();
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
        DisposeInputActions();
    }

    private void Update()
    {
        HandleMouseRotation();

        if (target != null)
        {
            cam.transform.LookAt(target);
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }

    private void SetupInputActions()
    {
        scrollAction = new(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(-ctx.ReadValue<Vector2>().y * zoomSpeed);

        middleButton = new(binding: "<Mouse>/middleButton");
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
            Enable();
        else
            Disable();
    }

    private void HandleMouseRotation()
    {
        if (!rotatingKBM || target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float rotationY = mouseDelta.x * rotateSensitivity * 0.5f * Time.deltaTime;
        float rotationX = -mouseDelta.y * rotateSensitivity * 0.5f * Time.deltaTime;

        transform.RotateAround(target.position, Vector3.up, rotationY);

        Vector3 direction = (cam.transform.position - target.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, target.position);
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z).normalized;

        float currentPitch = Vector3.SignedAngle(flatDir, direction, Vector3.Cross(flatDir, Vector3.up));
        float newPitch = Mathf.Clamp(currentPitch + rotationX, minPitch, maxPitch);
        float pitchDelta = newPitch - currentPitch;

        Vector3 rightAxis = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 newDir = Quaternion.AngleAxis(pitchDelta, rightAxis) * direction;

        cam.transform.position = target.position + newDir * distance;
        cam.transform.LookAt(target);
        offset = transform.position - target.position;
    }

    private void HandlePinch(float delta)
    {
        float distance = offset.magnitude - delta * zoomSpeed * 0.01f;
        distance = Mathf.Clamp(distance, minZoomDistance, maxZoomDistance);
        offset = offset.normalized * distance;
    }

    private void HandleRotate(float angleDelta)
    {
        float rotationAmount = angleDelta * rotateSensitivity * Time.deltaTime;
        transform.RotateAround(target.position, Vector3.up, rotationAmount);
        transform.LookAt(target);
        offset = transform.position - target.position;
    }

    private void HandleDrag(Vector2 delta)
    {
        float moveAmountX = -delta.y * Time.deltaTime * pitchSensitivity;

        Vector3 direction = (cam.transform.position - target.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, target.position);
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z).normalized;

        float currentPitch = Vector3.SignedAngle(flatDir, direction, Vector3.Cross(flatDir, Vector3.up));
        float newPitch = Mathf.Clamp(currentPitch + moveAmountX, minPitch, maxPitch);
        float pitchDelta = newPitch - currentPitch;

        Vector3 rightAxis = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 newDir = Quaternion.AngleAxis(pitchDelta, rightAxis) * direction;

        cam.transform.position = target.position + newDir * distance;
        cam.transform.LookAt(target);
        offset = transform.position - target.position;
    }

    private void CameraZoom(float increment)
    {
        float distance = offset.magnitude;
        distance = Mathf.Clamp(distance + increment, minZoomDistance, maxZoomDistance);
        offset = offset.normalized * distance;
    }

    private void Enable()
    {
        enabled = true;

        scrollAction?.Enable();
        middleButton?.Enable();
    }

    private void Disable()
    {
        enabled = false;

        scrollAction?.Disable();
        middleButton?.Disable();
    }
}
