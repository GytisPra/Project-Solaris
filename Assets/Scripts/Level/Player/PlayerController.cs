using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float movementDeadzone = 0.1f;

    private Vector2 moveInput;
    private Animator animator;
    private Transform cameraTransform;

    [SerializeField] private InputActionReference moveAction;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
        moveAction.action.Enable();
    }

    private void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
        moveAction.action.Disable();
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Gameplay)
            moveAction.action.Enable();
        else
        {
            StopMovement();
            moveAction.action.Disable();
        }
    }

    void FixedUpdate()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > movementDeadzone * movementDeadzone)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            transform.rotation = Quaternion.LookRotation(moveDirection);
            transform.Translate(Time.deltaTime * walkSpeed * moveDirection, Space.World);

            float inputStrength = Mathf.Clamp01(moveInput.magnitude);
            animator.SetFloat("speedPercent", inputStrength);
        }
        else
        {
            animator.SetFloat("speedPercent", 0f);
        }
    }

    private void StopMovement()
    {
        animator.SetFloat("speedPercent", 0f);
    }
}
