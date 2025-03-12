using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PinchScrollDetection : MonoBehaviour
{

    private CameraRotation cameraRotation;
    private float previousMagnitude;
    private int touchCount;

    public float speed = 10f;
    public float minDistance = 10f;
    public float maxDistance = 500f;
    void Start()
    {

        cameraRotation = Camera.main.GetComponent<CameraRotation>();

        // Mouse scroll
        InputAction scrollAction = new(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(-ctx.ReadValue<Vector2>().y * speed);

        // Pinch gesture
        InputAction touch0Contact = new
        (
           type: InputActionType.Button,
           binding: "<Touchscreen>/touch0/press"
        );
        touch0Contact.Enable();

        InputAction touch0Pos = new
        (
            type: InputActionType.Value,
            binding: "<Touchscreen>/touch0/position"
        );
        touch0Pos.Enable();


        InputAction touch1Contact = new
        (
           type: InputActionType.Button,
           binding: "<Touchscreen>/touch1/press"
        );
        touch1Contact.Enable();

        InputAction touch1Pos = new
        (
            type: InputActionType.Value,
            binding: "<Touchscreen>/touch1/position"
        );
        touch1Pos.Enable();

        touch0Contact.performed += _ => touchCount++;
        touch1Contact.performed += _ => touchCount++;

        touch0Contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
        };
        touch1Contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
        };

        touch1Pos.performed += _ =>
        {
            if (touchCount < 2)
            {
                return;
            }

            var magnitude = (touch0Pos.ReadValue<Vector2>() - touch1Pos.ReadValue<Vector2>()).magnitude;
            if (previousMagnitude == 0)
            {
                previousMagnitude = magnitude;
            }

            var difference = magnitude - previousMagnitude;
            previousMagnitude = magnitude;

            CameraZoom(-(difference * (speed * 0.5f)));
        };

    }

    private void CameraZoom(float increment)
    {
        cameraRotation.cameraDistance = Mathf.Clamp(cameraRotation.cameraDistance + increment, minDistance, maxDistance);
    }


}