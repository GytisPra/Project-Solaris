using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2.0f;

    private Vector2 touchStartPosition;
    private Vector2 touchDirection;
    private bool isMoving = false;
    private Animator animator;
    private int touchCount = 0;

    private InputAction keyboardAction;

    void Start()
    {
        animator = GetComponent<Animator>();

        InputAction touch0Contact = new(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        touch0Contact.Enable();
        touch0Contact.performed += _ => touchCount++;
        touch0Contact.canceled += _ => touchCount--;;

        InputAction touch1Contact = new(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        touch1Contact.Enable();
        touch1Contact.performed += _ => touchCount++;
        touch1Contact.canceled += _ => touchCount--;

        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp += OnFingerUp;

        keyboardAction = new InputAction(type: InputActionType.Value);
        keyboardAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");
        keyboardAction.Enable();
    }

    void FixedUpdate()
    {
        if (touchCount > 1)
        {
            animator.SetFloat("speedPercent", 0f);  // Reset speedPercent if multiple touches.
            return;
        }

        if (isMoving)
        {
            MovePlayer(touchDirection);
        }
        else
        {
            Vector2 keyboardInput = keyboardAction.ReadValue<Vector2>();

            if (keyboardInput != Vector2.zero)
            {
                MovePlayer(keyboardInput);
            }
            else
            {
                animator.SetFloat("speedPercent", 0f);
            }
        }
    }

    void OnFingerDown(Finger finger)
    {
        touchStartPosition = finger.screenPosition;
        isMoving = true;
        animator.SetFloat("speedPercent", 1f);
    }

    void OnFingerMove(Finger finger)
    {
        touchDirection = (finger.screenPosition - touchStartPosition).normalized;
    }

    void OnFingerUp(Finger finger)
    {
        isMoving = false;
        touchDirection = Vector2.zero;
        animator.SetFloat("speedPercent", 0f);
    }

    void MovePlayer(Vector2 normalizedInput)
    {
        if (normalizedInput != Vector2.zero)
        {
            Vector3 movementDirection = new(normalizedInput.x, 0, normalizedInput.y);
            transform.rotation = Quaternion.LookRotation(movementDirection);
            transform.Translate(Time.deltaTime * walkSpeed * movementDirection, Space.World);

            animator.SetFloat("speedPercent", 1f);
        }
    }

    void OnDestroy()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp -= OnFingerUp;
    }
}
