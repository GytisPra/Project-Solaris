using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float minFOV = 30f;
    public float maxFOV = 90f;
    public float zoomSpeed = 0.5f;
    public float pitchSensitivity = 10f;
    public float rotateSensitivity = 15f;
    public float maxPitch = 80f;
    public float minPitch = 10f;

    private float previousMagnitude;
    private int touchCount;
    private bool pinching;
    private bool moving;
    private bool rotating;
    private Vector2 previousTouchVector;

    private Camera cam;
    private InputAction touch0Contact;
    private InputAction touch0Pos;
    private InputAction touch0Delta;

    private InputAction touch1Contact;
    private InputAction touch1Pos;
    private InputAction touch1Delta;

    private InputAction scrollAction;

    void Start()
    {
        cam = Camera.main;
        EnhancedTouchSupport.Enable();

        scrollAction = new(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(-ctx.ReadValue<Vector2>().y * zoomSpeed);

        touch0Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        touch0Contact.Enable();
        touch0Contact.performed += _ => touchCount++;
        touch0Contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
            previousTouchVector = Vector2.zero;
        };

        touch0Pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        touch0Pos.Enable();

        touch0Delta = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/delta");
        touch0Delta.Enable();

        touch1Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        touch1Contact.Enable();
        touch1Contact.performed += _ => touchCount++;
        touch1Contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
            previousTouchVector = Vector2.zero;
        };

        touch1Pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        touch1Pos.Enable();

        touch1Delta = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/delta");
        touch1Delta.Enable();

        touch1Pos.performed += _ => DetectMotion();
    }

    void LateUpdate()
    {
        if (touchCount <= 1)
        {
            pinching = false;
            moving = false;
            rotating = false;
        }

        if (target)
        {
            transform.LookAt(target.transform, target.up);
        }
    }

    void DetectMotion()
    {
        Debug.Log($"pinching: {pinching} | moving: {moving} | rotating: {rotating}");

        if (touchCount < 2 || cam == null)
            return;

        Vector2 delta0 = touch0Delta.ReadValue<Vector2>();
        Vector2 delta1 = touch1Delta.ReadValue<Vector2>();

        Vector2 pos0 = touch0Pos.ReadValue<Vector2>();
        Vector2 pos1 = touch1Pos.ReadValue<Vector2>();

        // Calculate vectors between fingers
        Vector2 currentVector = pos1 - pos0;
        float angleDelta = 0f;

        // Calculate rotation angle between frames
        if (previousTouchVector != Vector2.zero)
        {
            angleDelta = Vector2.SignedAngle(previousTouchVector, currentVector);
        }
        previousTouchVector = currentVector;
        float magnitude = (pos0 - pos1).magnitude;

        if (previousMagnitude == 0)
        {
            previousMagnitude = magnitude;
        }

        float magnitudeDifference = magnitude - previousMagnitude;
        previousMagnitude = magnitude;


        if (!pinching && !moving && !rotating)
        {
            bool isZooming = Mathf.Abs(magnitudeDifference) > 50f;
            bool isRotating = Mathf.Abs(angleDelta) > 4f;
            bool isMoving = Vector2.Dot(delta0.normalized, delta1.normalized) > 0.2f;

            if (isZooming)
            {
                pinching = true;
            }
            else if (isRotating)
            {
                rotating = true;
            }
            else if (isMoving)
            {
                moving = true;
            }
        }


        if (rotating)
        {
            float rotationAmount = angleDelta * rotateSensitivity * Time.deltaTime;

            transform.RotateAround(target.position, Vector3.up, rotationAmount);

            transform.LookAt(target);
        }
        else if (pinching)
        {
            cam.fieldOfView -= magnitudeDifference * zoomSpeed * 0.25f;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
        else if (moving)
        {
            float moveAmount = -delta0.y * Time.deltaTime * pitchSensitivity;

            Vector3 directionToCamera = (cam.transform.position - target.transform.position).normalized;
            float distanceToTarget = Vector3.Distance(cam.transform.position, target.transform.position);

            Vector3 flatDirection = new Vector3(directionToCamera.x, 0, directionToCamera.z).normalized;
            float currentPitch = Vector3.SignedAngle(flatDirection, directionToCamera, Vector3.Cross(flatDirection, Vector3.up));

            float newPitch = Mathf.Clamp(currentPitch + moveAmount, minPitch, maxPitch);
            float pitchDelta = newPitch - currentPitch;

            Vector3 rightAxis = Vector3.Cross(directionToCamera, Vector3.up).normalized;
            Vector3 newDirection = Quaternion.AngleAxis(pitchDelta, rightAxis) * directionToCamera;

            Vector3 targetPosition = target.transform.position + newDirection * distanceToTarget;
            cam.transform.position = targetPosition;

            cam.transform.LookAt(target.transform);
        }
    }

    private void CameraZoom(float increment)
    {
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + increment, minFOV, maxFOV);
    }

    void OnDestroy()
    {
        scrollAction.Dispose();

        touch0Contact.Dispose();
        touch0Pos.Dispose();
        touch0Delta.Dispose();

        touch1Contact.Dispose();
        touch1Pos.Dispose();
        touch1Delta.Dispose();
    }
}
