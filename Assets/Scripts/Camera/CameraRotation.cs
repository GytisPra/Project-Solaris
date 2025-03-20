using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class CameraRotation : MonoBehaviour
{
    public float sensitivity = 5f;
    public float cameraDistance = 200f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    public float transitionSpeed = 200f;
    public float resetCameraDistance;
    public GameObject target;
    public GameObject sun;
    public bool disableInput = false;

    [SerializeField] private float rotationDrag;
    [SerializeField] private float transitionDuration = 2f;

    private float rotationY = 0f;
    private float rotationX = 0f;
    private float rotationVelocityX;
    private float rotationVelocityY;
    private float lastPlanetRotationY;

    private bool smoothTransition;
    private float transitionProgress = 0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float minAllowedDistance;

    private InputAction touch;
    private InputAction click;
    private InputAction mouseDelta;

    private void Awake()
    {
        InputAction touchAction = new(
            type: InputActionType.Value,
            binding: "<Touchscreen>/primaryTouch"
        );
        touchAction.Enable();

        InputAction clickAction = new(
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        );
        clickAction.Enable();

        InputAction mouseDeltaAction = new(
            type: InputActionType.Value,
            binding: "<Mouse>/delta"
        );
        mouseDeltaAction.Enable();

        touch = touchAction;
        click = clickAction;
        mouseDelta = mouseDeltaAction;
    }

    private void OnDisable()
    {
        click.Disable();
        mouseDelta.Disable();
        touch.Disable();
    }

    void Start()
    {
        if (target != null)
        {
            Vector3 direction = new(0, Mathf.Sin(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance, -Mathf.Cos(Mathf.Deg2Rad * maxVerticalAngle) * cameraDistance);
            transform.position = target.transform.position + direction;

            transform.LookAt(target.transform);

            rotationX = 50f;
            rotationY = 0f;
        }
        else
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }
    }

    private float GetLookRotationY(GameObject gameObject)
    {
        Vector3 direction = (target.transform.position - gameObject.transform.position).normalized;
        Vector3 newCameraPosition = target.transform.position - direction * cameraDistance;

        return Quaternion.LookRotation(target.transform.position - newCameraPosition).eulerAngles.y;
    }

    private float GetLookRotationX(GameObject gameObject)
    {
        Vector3 direction = (target.transform.position - gameObject.transform.position).normalized;
        Vector3 newCameraPosition = target.transform.position - direction * cameraDistance;

        return Quaternion.LookRotation(target.transform.position - newCameraPosition).eulerAngles.x;
    }
    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (EventSystem.current.IsPointerOverGameObject() && click.ReadValue<float>() > 0)
            return true;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return true;
        }

        return false;
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
        if (target == null)
        {
            return;
        }

        bool isPointerOverUI = IsPointerOverUI();

        TouchState touchState = touch.ReadValue<TouchState>();
        Vector2 mouseDelta = this.mouseDelta.ReadValue<Vector2>();

        bool isTouching = touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began;
        bool isClicking = click.ReadValue<float>() > 0;

        bool isUserControlling = isClicking || isTouching;

        float planetRotationY = target.transform.rotation.eulerAngles.y;

        if (isUserControlling && !disableInput && !isPointerOverUI)
        {
            UpdateRotation(ref rotationVelocityY, isClicking ? mouseDelta.x : touchState.delta.x);
            UpdateRotation(ref rotationVelocityX, isClicking ? mouseDelta.y : touchState.delta.y, true);

            lastPlanetRotationY = planetRotationY;
        }
        else if (Mathf.Abs(rotationVelocityY) > 0f || Mathf.Abs(rotationVelocityX) > 0f)
        {
            UpdateRotation(ref rotationVelocityY, 0);
            UpdateRotation(ref rotationVelocityX, 0);

            lastPlanetRotationY = planetRotationY;
        }
        else
        {
            float deltaRotationY = planetRotationY - lastPlanetRotationY;
            rotationY += deltaRotationY;

            lastPlanetRotationY = planetRotationY;
        }


        rotationY += rotationVelocityY;
        rotationX = Mathf.Clamp(rotationX + rotationVelocityX, minVerticalAngle, maxVerticalAngle);

        if (target != null)
        {
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -cameraDistance);
            Vector3 targetPosition = target.transform.position + direction;

            if (smoothTransition)
            {
                transitionProgress += Time.deltaTime / transitionDuration;
                transitionProgress = Mathf.Clamp01(transitionProgress);
                float distance = Vector3.Distance(transform.position, targetPosition);

                if (distance < 0.07f)
                {
                    transform.position = targetPosition;
                    smoothTransition = false;
                    transitionProgress = 0f;
                } 
                else
                {
                    float smoothSpeed = Mathf.SmoothStep(0f, transitionSpeed, transitionProgress);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

                    float currentDistance = Vector3.Distance(transform.position, target.transform.position);
                    if (currentDistance < minAllowedDistance)
                    {
                        Vector3 newDirection = (transform.position - target.transform.position).normalized * minAllowedDistance;
                        transform.position = target.transform.position + newDirection;
                    }
                }
            }
            else
            {
                transform.position = targetPosition;
            }
            transform.LookAt(target.transform);
        }
    }

    public void ResetCamera()
    {
        target = null;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }

    public void ResetCameraOnCurrentTarget()
    {
        if (target == null || sun == null) return;

        cameraDistance = minAllowedDistance = Utils.GetRadius(target) * 10.0f;

        rotationY = GetLookRotationY(sun);
        rotationX = 0;

        smoothTransition = true;
    }

    public void SetTargetObject(GameObject newTarget)
    {
        if (newTarget == target)
        {
            return;
        }

        if (newTarget.TryGetComponent<LoadLevelsOnPlanet>(out var loadLevelsOnNewTarget))
        {
            loadLevelsOnNewTarget.ShowLevels(true);
        }

        if (target != null && target.TryGetComponent<LoadLevelsOnPlanet>(out var loadLevelsOnOldTarget))
        {
            loadLevelsOnOldTarget.ShowLevels(false);
        }

        target = newTarget;
        cameraDistance = Utils.GetRadius(newTarget) * 10.0f;
        minAllowedDistance = cameraDistance;

        rotationX = 0;
        rotationY = GetLookRotationY(sun);

        smoothTransition = true;
    }
    public void GoToLevel(GameObject level)
    {
        smoothTransition = true;
        rotationX = GetLookRotationX(level);
        rotationY = GetLookRotationY(level);
    }
    public string GetCurrentTargetName()
    {
        if (target == null)
        {
            return "";
        }

        return target.name;
    }

    public GameObject GetCurrentTarget()
    {
        return target;
    }

    public void DisableInput()
    {
        disableInput = true;
    }

    public void EnableInput()
    {
        disableInput = false;
    }
}
