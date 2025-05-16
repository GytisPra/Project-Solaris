using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2.0f;
    public float movementDeadzone = 0.1f;

    private Vector2 touchStartPosition;
    private Vector2 touchDirection;
    private bool isTouching = false;
    private bool isPerformingGesture = false;
    private Animator animator;
    private Transform cameraTransform;
    private InputAction keyboardAction;
    void Start()
    {
        cameraTransform = Camera.main.gameObject.transform;

        animator = GetComponent<Animator>();

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

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
        //TouchGestureManager.Instance.OnGestureActive += HandleGestureActive;
    }
    void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
        TouchGestureManager.Instance.OnGestureActive -= HandleGestureActive;

        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp -= OnFingerUp;
        keyboardAction.Dispose();
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    void FixedUpdate()
    {
        if (isPerformingGesture)
        {
            animator.SetFloat("speedPercent", 0f);
        }
        else if (isTouching)
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

    private void HandleGestureActive(bool isPerforming)
    {
        isPerformingGesture = isPerforming;
    }

    void OnFingerDown(Finger finger)
    {
        touchStartPosition = finger.screenPosition;
        isTouching = true;
    }

    void OnFingerMove(Finger finger)
    {
        touchDirection = (finger.screenPosition - touchStartPosition).normalized;
    }

    void OnFingerUp(Finger finger)
    {
        isTouching = false;
        touchDirection = Vector2.zero;
        animator.SetFloat("speedPercent", 0f);
    }

    void MovePlayer(Vector2 normalizedInput)
    {
        if (normalizedInput.magnitude > movementDeadzone)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * normalizedInput.y + cameraRight * normalizedInput.x;

            transform.rotation = Quaternion.LookRotation(moveDirection);
            transform.Translate(Time.deltaTime * walkSpeed * moveDirection, Space.World);

            animator.SetFloat("speedPercent", 1f);
        }
        else
        {
            animator.SetFloat("speedPercent", 0f);
        }
    }

    public void Disable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp -= OnFingerUp;

        keyboardAction.Disable();
    }
    public void Enable()
    {
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp += OnFingerUp;

        keyboardAction.Enable();
    }
}
