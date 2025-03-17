using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System.Collections;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraRotation : MonoBehaviour
{
    public float sensitivity = 5f;
    public float cameraDistance = 200f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    public float transitionSpeed = 200f;
    public float rotationSpeed = 50f;
    public float resetCameraDistance;
    public GameObject target;
    public bool disableInput = false;

    [SerializeField] private float rotationDrag;

    private float rotationY = 0f;
    private float rotationX = 0f;
    private float rotationVelocityX;
    private float rotationVelocityY;

    //private bool smoothTransition;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

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

        TouchState touchState = touch.ReadValue<TouchState>();
        Vector2 mouseDeltaDelta = mouseDelta.ReadValue<Vector2>();

        bool isTouching = touchState.isInProgress && touchState.isPrimaryTouch && touchState.phase != TouchPhase.Began;
        bool isClicking = click.ReadValue<float>() > 0;

        if (isClicking && !disableInput)
        {
            UpdateRotation(ref rotationVelocityY, mouseDeltaDelta.x);
            UpdateRotation(ref rotationVelocityX, mouseDeltaDelta.y, true);
        }
        else if (isTouching && !disableInput)
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

        if (target != null)
        {
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            Vector3 direction = rotation * new Vector3(0, 0, -cameraDistance);
            Vector3 targetPosition = target.transform.position + direction;

            //if (smoothTransition)
            //{
            //    transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

            //    Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //    if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            //    {
            //        transform.SetPositionAndRotation(targetPosition, targetRotation);
            //        smoothTransition = false;
            //    }
            //}
            //else
            //{
            transform.position = targetPosition;
            transform.LookAt(target.transform);
            //}
        }
    }

    public void ResetCamera()
    {
        target = null;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }

    public void ResetCameraOnCurrentTarget()
    {
        cameraDistance = Utils.GetSphereRadius(target) * 10.0f;
        //smoothTransition = true;

        rotationX = 0;
        rotationY = 0;
    }

    public void SetTargetObject(GameObject newTarget)
    {
        target = newTarget;
        cameraDistance = Utils.GetSphereRadius(newTarget) * 10.0f;

        //smoothTransition = true;
        rotationX = 0;
    }
    public string GetCurrentTarget()
    {
        if (target == null)
        {
            return "";
        }

        return target.name;
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
