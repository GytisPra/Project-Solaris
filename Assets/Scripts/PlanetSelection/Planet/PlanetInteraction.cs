using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlanetInteraction : MonoBehaviour
{
    public LayerMask layerMask;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public CameraRotation cameraRotation;
    public bool disabled;

    private InputAction touch;
    private InputAction interactionPosition;
    private InputAction click;

    private void Awake()
    {
        touch = new(
            type: InputActionType.Value,
            binding: "<Touchscreen>/primaryTouch"
        );
        touch.Enable();

        click = new(
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        );
        click.Enable();

        interactionPosition = new(
            type: InputActionType.Value,
            binding: "<Pointer>/position"
        );
        interactionPosition.Enable();
    }

    private void OnDisable()
    {
        click.Disable();
        touch.Disable();
        interactionPosition.Disable();
    }
    void Update()
    {
        TouchState touchState = touch.ReadValue<TouchState>();

        if ((click.triggered || touchState.phase == TouchPhase.Began) && !disabled)
        {
            var interactionLocation = touchState.phase == TouchPhase.Began ?
                                        touchState.position : interactionPosition.ReadValue<Vector2>();

            if (interactionLocation == null || Camera.main == null)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(interactionLocation);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.layer == LayerMask.NameToLayer("Planet"))
                {
                    return;
                }

                LevelData levelData = hitObject.GetComponent<LevelData>();
                SpriteRenderer fillRenderer = hitObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

                planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                planetSelectionUIManager.SetPlanetUICanvasActive(false);
                planetSelectionUIManager.OpenLevelUI(levelData, fillRenderer);
                cameraRotation.GoToLevel(hitObject);
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
