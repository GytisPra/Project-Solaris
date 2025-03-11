using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraRotation : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public float sensitivity = 5f;
    public float cameraDistance = 200f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [SerializeField] private GameObject planet;
    [SerializeField] private Camera planetCam;
    [SerializeField] private float rotationDrag;

    private float rotationY = 0f;
    private float rotationX = 0f;
    private float rotationVelocityX;
    private float rotationVelocityY;
    private InputActionMap uiActionMap;
    private InputAction touch;

    private void Awake()
    {
        uiActionMap = inputActionAsset.FindActionMap("UI", true);
        uiActionMap.Enable();

        touch = uiActionMap.FindAction("Touch", true);
        touch.Enable();
    }

    private void OnEnable()
    {
        uiActionMap?.Enable();
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
        touch?.Disable();
    }

    void Start()
    {
        if (planet != null && planetCam != null)
        {
            Vector3 direction = new(0, 0, -cameraDistance);
            planetCam.transform.position = planet.transform.position + direction;
            planetCam.transform.LookAt(planet.transform);
        }
    }
    void Update()
    {
        /* TODO: 
         * remove angle clamping (allow to flip). 
         * maybe add ability to zoom?
         * Add ability to go to a level (will be displayed on the surface)
        */

        TouchState touchState = touch.ReadValue<TouchState>();


        if (touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began) 
        {
            if (touchState.delta.x != 0)
            {
                // If the finger is moving apply the movement
                rotationVelocityY = touchState.delta.x * Time.deltaTime * sensitivity;
            }
            else if (touchState.delta.x == 0 && rotationDrag > 0 && Mathf.Abs(rotationVelocityY) > 1e-2)
            {
                // If the finger is not moving apply drag
                rotationVelocityY = Mathf.Lerp(rotationVelocityY, 0, Time.deltaTime * rotationDrag);
            }
            else if (rotationDrag > 0)
            {
                // Set to zero when the velocity is less than 1e-2
                rotationVelocityY = 0;
            }

            if (touchState.delta.y != 0)
            {
                rotationVelocityX = -touchState.delta.y * Time.deltaTime * sensitivity;
            }
            else if (touchState.delta.y == 0 && rotationDrag > 0 && Mathf.Abs(rotationVelocityX) > 1e-2)
            {
                rotationVelocityX = Mathf.Lerp(rotationVelocityX, 0, Time.deltaTime * rotationDrag);
            }
            else if (rotationDrag > 0)
            {
                rotationVelocityX = 0;
            }

        }
        // Apply drag (same way as before) when the screen is not being touched
        else if (rotationDrag > 0)
        {
            if (Mathf.Abs(rotationVelocityX) > 1e-2)
            {
                rotationVelocityX = Mathf.Lerp(rotationVelocityX, 0, Time.deltaTime * rotationDrag);
            }
            else
            {
                rotationVelocityX = 0;
            }

            if (Mathf.Abs(rotationVelocityY) > 1e-2)
            {
                rotationVelocityY = Mathf.Lerp(rotationVelocityY, 0, Time.deltaTime * rotationDrag);
            }
            else
            {
                rotationVelocityY = 0;
            }
        }

        rotationY += rotationVelocityY;
        rotationX += rotationVelocityX;

        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        if (planet != null && planetCam != null)
        {
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -cameraDistance);
            planetCam.transform.position = planet.transform.position + direction;
            planetCam.transform.LookAt(planet.transform);
        }
    }
}
