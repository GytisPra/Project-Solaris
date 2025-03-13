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

    private InputActionMap uiActionMap;
    private InputAction touch;
    private InputAction interactionPosition;
    private InputAction click;
    private GameObject prevHitObject;

    private CameraRotation cameraRotation;
    private bool zoomIn = true;

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

        if (click.triggered || touchState.phase == TouchPhase.Began)
        {
            var interactionLocation = touchState.phase == TouchPhase.Began ?
                                        touchState.position : interactionPosition.ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(interactionLocation);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                
                GameObject hitObject = hit.collider.gameObject;

                Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                if (cameraRotation != null && cameraRotation.rotateAround.name != hitObject.name)
                {
                    cameraRotation.rotateAround = hitObject;

                    Bounds bounds = hit.collider.bounds;
                    float objectSize = bounds.extents.magnitude;

                    float offset = 30.0f;
                    cameraRotation.cameraDistance = objectSize + offset;
                    zoomIn = !zoomIn;
                    
                    // if (hitObject.TryGetComponent<OrbitRing>(out var orbitRing)) {
                    //     orbitRing.SetLineVisibility(false);
                    //     if (prevHitObject != null && prevHitObject.TryGetComponent<OrbitRing>(out var prevOrbitRing)) {
                    //         prevOrbitRing.SetLineVisibility(true);
                    //     }
                    // }

                    if (planetSelectionUIManager != null) {
                        planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                        planetSelectionUIManager.SetPlanetUICanvasActive(true);
                    } else {
                        Debug.LogError("Planet selection UI manager not assigned in inspector!");
                    }

                    Debug.Log("New rotation target: " + hit.collider.gameObject.name);
                }

                prevHitObject = hitObject;
            }
            else
            {
                Debug.Log("Missed!");
            }
        }
    }
}
