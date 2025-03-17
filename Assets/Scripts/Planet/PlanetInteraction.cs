using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlanetInteraction : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public Camera solarSystemCamera;
    public LayerMask layerMask;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public bool disabled;

    private InputActionMap uiActionMap;
    private InputAction touch;
    private InputAction interactionPosition;
    private InputAction click;

    private CameraRotation cameraRotation;

    private void Awake()
    {
        uiActionMap = inputActionAsset.FindActionMap("UI", true);
        uiActionMap.Enable();

        click = uiActionMap.FindAction("Click", true);
        click.Enable();

        touch = uiActionMap.FindAction("Touch", true);
        touch.Enable();

        interactionPosition = uiActionMap.FindAction("InteractionPosition", true);
        interactionPosition.Enable();

        if (solarSystemCamera != null)
        {
            cameraRotation = solarSystemCamera.GetComponent<CameraRotation>();
        }
        else
        {
            Debug.LogError("Solar system camera is not assigned in the Inspector!");
        }
    }

    private void OnEnable()
    {
        uiActionMap?.Enable();
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
        click?.Disable();
        touch?.Disable();
        interactionPosition?.Disable();
    }
    void Update()
    {
        TouchState touchState = touch.ReadValue<TouchState>();

        if ((click.triggered || touchState.phase == TouchPhase.Began) && !disabled)
        {
            var interactionLocation = touchState.phase == TouchPhase.Began ?
                                        touchState.position : interactionPosition.ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(interactionLocation);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.layer == LayerMask.NameToLayer("Planet")) {
                    return;
                }

                if (cameraRotation != null)
                {
                    if (cameraRotation.GetCurrentTarget() == hitObject.name) {
                        return;
                    }

                    LevelData levelData = hitObject.GetComponent<LevelData>();

                    if (planetSelectionUIManager != null)
                    {
                        planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                        planetSelectionUIManager.SetPlanetUICanvasActive(false);
                        planetSelectionUIManager.OpenLevelUI(levelData);
                    }
                    else
                    {
                        Debug.LogError("Planet selection UI manager not assigned in inspector!");
                    }
                }
            }
        }
    }

    public void Disable() 
    {
        disabled = true; 
    }

    public void Enable() 
    {
        disabled = false; 
    }
}
