using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float minFOV = 30f;
    public float maxFOV = 90f;
    public float zoomSpeed = 0.5f;
    private float previousMagnitude;
    private int touchCount;

    private Camera cam;
    private InputAction touch0Contact;
    private InputAction touch0Pos;
    private InputAction touch1Contact;
    private InputAction touch1Pos;

    void Start()
    {
        cam = Camera.main;
        EnhancedTouchSupport.Enable();

        touch0Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        touch0Contact.Enable();
        touch0Contact.performed += _ => touchCount++;
        touch0Contact.canceled += _ => { touchCount--; previousMagnitude = 0; };

        touch0Pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        touch0Pos.Enable();

        touch1Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        touch1Contact.Enable();
        touch1Contact.performed += _ => touchCount++;
        touch1Contact.canceled += _ => { touchCount--; previousMagnitude = 0; };

        touch1Pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        touch1Pos.Enable();

        touch1Pos.performed += _ => DetectPinch();
    }

    void LateUpdate()
    {
        if (target)
        {
            transform.LookAt(target.transform);
        }
    }

    void DetectPinch()
    {
        if (touchCount < 2 || cam == null) return;

        Vector2 pos0 = touch0Pos.ReadValue<Vector2>();
        Vector2 pos1 = touch1Pos.ReadValue<Vector2>();
        float magnitude = (pos0 - pos1).magnitude;

        if (previousMagnitude == 0)
            previousMagnitude = magnitude;

        float difference = magnitude - previousMagnitude;
        previousMagnitude = magnitude;

        cam.fieldOfView -= difference * zoomSpeed * 0.25f;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
    }

    void OnDestroy()
    {
        touch0Contact.Dispose();
        touch0Pos.Dispose();
        touch1Contact.Dispose();
        touch1Pos.Dispose();
    }
}
