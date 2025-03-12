using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraRotation : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public bool disableMouseInput;
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
    private InputAction click;
    private InputAction mouseDelta;

    private void Awake()
    {
        uiActionMap = inputActionAsset.FindActionMap("UI", true);
        uiActionMap.Enable();

        touch = uiActionMap.FindAction("Touch", true);
        touch.Enable();

        click = uiActionMap.FindAction("Click", true);
        click.Enable();

        mouseDelta = uiActionMap.FindAction("MouseDelta", true);
        mouseDelta.Enable();
    }

    private void OnEnable()
    {
        uiActionMap?.Enable();
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
        click?.Disable();
        mouseDelta?.Disable();
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

    void UpdateRotation(ref float velocity, float input, bool invert = false)
    {
        if (input != 0)
        {
            velocity = (invert ? -input : input) * Time.deltaTime * sensitivity;
        }
        else if (rotationDrag > 0)
        {
            velocity = Mathf.Abs(velocity) > 1e-2 ? Mathf.Lerp(velocity, 0, Time.deltaTime * rotationDrag) : 0;
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
        Vector2 mouseDeltaDelta = mouseDelta.ReadValue<Vector2>();

        Debug.Log($"Move base on touch input?: {touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began}");
        Debug.Log($"Move base on click input?: {click.ReadValue<float>() > 0}");

        if (click.ReadValue<float>() > 0)
        {
            UpdateRotation(ref rotationVelocityY, mouseDeltaDelta.x);
            UpdateRotation(ref rotationVelocityX, mouseDeltaDelta.y, true);
        }
        else if (touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began)
        {
            UpdateRotation(ref rotationVelocityY, touchState.delta.x);
            UpdateRotation(ref rotationVelocityX, touchState.delta.y, true);
        }
        else if (rotationDrag > 0)
        {
            UpdateRotation(ref rotationVelocityX, 0);
            UpdateRotation(ref rotationVelocityY, 0);
        }

        rotationY += rotationVelocityY;
        rotationX = Mathf.Clamp(rotationX + rotationVelocityX, minVerticalAngle, maxVerticalAngle);

        if (planet != null && planetCam != null)
        {
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -cameraDistance);
            planetCam.transform.position = planet.transform.position + direction;
            planetCam.transform.LookAt(planet.transform);
        }
    }
}
