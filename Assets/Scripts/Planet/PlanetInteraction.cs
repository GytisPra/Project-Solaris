using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlanetInteraction : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    // [SerializeField] private float rotationSpeed = 5f;
    // [SerializeField] private GameObject planet;

    private InputActionMap uiActionMap;
    private InputAction touch;
    private InputAction click;
    // private Vector3 targetLookAt; // Target position to look at
    // private bool shouldRotate = false;

    private void Awake()
    {
        uiActionMap = inputActionAsset.FindActionMap("UI", true);
        uiActionMap.Enable();

        click = uiActionMap.FindAction("Click", true);
        click.Enable();

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
        click?.Disable();
        touch?.Disable();
    }

    void Start()
    {

    }
    void Update()
    {
        TouchState touchState = touch.ReadValue<TouchState>();

        if (click.triggered && touchState.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchState.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.name != gameObject.name)
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.black;
                }


                // targetLookAt = hit.point;
                // shouldRotate = true;
            }
            else
            {
                Debug.Log("Missed!");
            }
        }

        // if (shouldRotate)
        // {
        //     RotateCameraToTarget();
        // }
    }

    // void RotateCameraToTarget()
    // {
    //     if (Camera.main != null && planet != null)
    //     {
    //         Vector3 directionToTarget = targetLookAt - Camera.main.transform.position;
    //         Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

    //         Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    //         if (Quaternion.Angle(Camera.main.transform.rotation, targetRotation) < 0.1f)
    //         {
    //             shouldRotate = false;
    //         }
    //     }
    // }
}
