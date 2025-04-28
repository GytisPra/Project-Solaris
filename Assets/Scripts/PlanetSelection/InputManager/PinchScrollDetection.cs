using UnityEngine;
using UnityEngine.InputSystem;

public class PinchScrollDetection : MonoBehaviour
{

    private CameraRotation cameraRotation;

    private float previousMagnitude;
    private int touchCount;
    private string prevTarget = "";

    private InputAction scrollAction;
    private InputAction touch0Contact;
    private InputAction touch0Pos;
    private InputAction touch1Contact;
    private InputAction touch1Pos;

    public GameObject target;
    public float scroolSpeed = 4.75f;
    public float pinchSpeed = 2.5f;
    public float minDistance = 10f;
    public float maxDistance = 500f;

    private void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;

        scrollAction.Dispose();

        touch0Contact.Dispose();
        touch0Pos.Dispose();

        touch1Contact.Dispose();
        touch1Pos.Dispose();
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();

        // Mouse scroll
        scrollAction = new(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(-ctx.ReadValue<Vector2>().y * scroolSpeed);

        // Pinch gesture
        touch0Contact = new
        (
           type: InputActionType.Button,
           binding: "<Touchscreen>/touch0/press"
        );
        touch0Contact.Enable();

        touch0Pos = new
        (
            type: InputActionType.Value,
            binding: "<Touchscreen>/touch0/position"
        );
        touch0Pos.Enable();


        touch1Contact = new
        (
           type: InputActionType.Button,
           binding: "<Touchscreen>/touch1/press"
        );
        touch1Contact.Enable();

        touch1Pos = new
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

            CameraZoom(-(difference * (pinchSpeed * 0.25f)));
        };

    }

    private void CameraZoom(float increment)
    {
        if (cameraRotation != null && cameraRotation.enabled)
        {
            cameraRotation.cameraDistance = Mathf.Clamp(cameraRotation.cameraDistance + increment, minDistance, maxDistance);
        }
    }

    void Update()
    {
        if (prevTarget == cameraRotation.GetCurrentTargetName())
        {
            return;
        }

        minDistance = Utils.GetRadius(cameraRotation.GetCurrentTarget()) * 2.5f;
        prevTarget = cameraRotation.GetCurrentTargetName();
    }

    private void Enable()
    {
        scrollAction.Enable();

        touch0Contact.Enable();
        touch0Pos.Enable();

        touch1Contact.Enable();
        touch1Pos.Enable();
    }
    private void Disable()
    {
        scrollAction.Disable();

        touch0Contact.Disable();
        touch0Pos.Disable();

        touch1Contact.Disable();
        touch1Pos.Disable();
    }
}