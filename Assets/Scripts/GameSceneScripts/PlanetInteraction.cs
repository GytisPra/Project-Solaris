using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class PlanetInteraction : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    public float smoothSpeed = 10f;
    public float sensitivity = 10f;
    private float rotationY = 0f;

    private InputActionMap uiActionMap;
    private InputAction touchAction;
    private InputAction touchInfo;
    
    private void Awake()
    {
        // Access the "UI" action map
        uiActionMap = inputActionAsset.FindActionMap("UI", true);

        // Enable the action map to start listening for inputs
        uiActionMap.Enable();

        touchAction = uiActionMap.FindAction("TouchAction", true);
        touchInfo = uiActionMap.FindAction("touchInfo", true);


        touchAction.Enable();
        touchInfo.Enable();
    }

    private void OnEnable()
    {
        uiActionMap?.Enable();
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
        touchAction?.Disable();
        touchInfo?.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (touchAction.ReadValue<float>() > 0)
        {
            TouchState touch = touchInfo.ReadValue<TouchState>();
            if (touch.phase == TouchPhase.Moved)
            {
                rotationY += -touch.delta.x * Time.deltaTime * sensitivity;
            }
        }

        Quaternion targetRotation = Quaternion.AngleAxis(rotationY, Vector3.up);


        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
