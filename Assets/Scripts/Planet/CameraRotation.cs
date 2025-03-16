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
    public GameObject resetToTarget;
    public float resetCameraDistance;

    public GameObject rotateAround;
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
        if (rotateAround != null)
        {
            Vector3 direction = new(0, Mathf.Sin(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance, -Mathf.Cos(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance);
            transform.position = rotateAround.transform.position + direction;

            transform.LookAt(rotateAround.transform);

            rotationX = 50f;
            rotationY = 0f;
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
        TouchState touchState = touch.ReadValue<TouchState>();
        Vector2 mouseDeltaDelta = mouseDelta.ReadValue<Vector2>();

        bool isTouching = touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began;
        bool isClicking = click.ReadValue<float>() > 0;

        if (isClicking)
        {
            UpdateRotation(ref rotationVelocityY, mouseDeltaDelta.x);
            UpdateRotation(ref rotationVelocityX, mouseDeltaDelta.y, true);
        }
        else if (isTouching)
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


        if (rotateAround != null)
        {
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -cameraDistance);
            transform.position = rotateAround.transform.position + direction;
            transform.LookAt(rotateAround.transform);
        }
    }

    public void ResetCamera()
    {
        rotateAround = resetToTarget;
        cameraDistance = resetCameraDistance;

        Vector3 direction = new(0, Mathf.Sin(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance, -Mathf.Cos(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance);
        transform.position = rotateAround.transform.position + direction;

        transform.LookAt(rotateAround.transform);

        rotationX = 90f;
        rotationY = 0f;
    }

    public void ResetCameraOnCurrentTarget()
    {
        cameraDistance = Utils.GetSphereRadius(rotateAround) * 10.0f;

        rotationX = 0;
        rotationY = 0;
    }

    public void SetTargetObject(GameObject target)
    {
        rotateAround = target;
        cameraDistance = Utils.GetSphereRadius(target) * 10.0f;

        rotationX = 0;
        rotationY = 0;
    }

    public string GetCurrentTarget()
    {
        return rotateAround.name;
    }
}
