using UnityEngine;
using UnityEngine.InputSystem;

public class SphereClickGrow : MonoBehaviour
{
    public InputActionAsset inputActionAsset; // Drag your InputActionAsset in the inspector, or load it programmatically.

    private InputActionMap uiActionMap;
    private InputAction clickAction;
    private InputAction clickLocation;

    private Vector3 initialScale;
    public float scaleMultiplier = 1.5f;
    public float growthSpeed = 0.5f;
    private bool isHeld = false;
    private Vector3 currentScale;

    private void Awake()
    {
        // Ensure inputActionAsset is assigned (or you can load it programmatically)

        // Access the "UI" action map
        uiActionMap = inputActionAsset.FindActionMap("UI", true);

        // Enable the action map to start listening for inputs
        uiActionMap.Enable();

        clickAction = uiActionMap.FindAction("Click", true);
        clickLocation = uiActionMap.FindAction("TouchPosition", true);

        clickLocation.Enable();
        clickAction.Enable();
    }

    private void OnEnable()
    {
        uiActionMap?.Enable();
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
        clickAction?.Disable();
        clickLocation?.Disable();
    }

    private void Start()
    {
        initialScale = transform.localScale;
        currentScale = initialScale;
    }

    private void Update()
    {
        // Check if the "Click" action is triggered
        if (clickAction.ReadValue<float>() > 0)
        {
            Vector2 touchPos = clickLocation.ReadValue<Vector2>();
             
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            // If the ray hits the sphere
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the touched object is the sphere
                if (hit.transform == transform)
                {
                    // Start scaling the sphere
                    if (!isHeld)
                    {
                        isHeld = true;
                    }

                    // Gradually increase the size of the sphere
                    currentScale = Vector3.Lerp(currentScale, initialScale * scaleMultiplier, growthSpeed * Time.deltaTime);
                    transform.localScale = currentScale;
                }
            }
        }
        else
        {
            if (isHeld)
            {
                isHeld = false;
                currentScale = initialScale;
                transform.localScale = currentScale;
            }
        }
    }
}
