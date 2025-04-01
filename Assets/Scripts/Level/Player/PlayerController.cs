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
    private new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp += OnFingerUp;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Touch.activeTouches.Count > 1)
        {
            animator.SetFloat("speedPercent", 0f);  // Reset speedPercent if there are multiple touches.
            return;
        }

        if (isMoving)
        {
            MovePlayer();
        }
    }

    void OnFingerDown(Finger finger)
    {
        touchStartPosition = finger.screenPosition;
        isMoving = true;
        animator.SetFloat("speedPercent", 1f);  // Set speed to walking when the finger is down.
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

    void MovePlayer()
    {
        if (touchDirection != Vector2.zero)
        {
            Vector3 movementDirection = new(touchDirection.x, 0, touchDirection.y);
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
