using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float turnSpeed = 10f;           // how fast we turn to face move direction
    public float movementDeadzone = 0.1f;

    [SerializeField] private InputActionReference moveAction;

    private Vector2 moveInput;
    private Animator animator;
    private Transform cameraTransform;

    void Awake()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
        moveAction.action.Enable();
    }

    void OnDestroy()
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
            moveAction.action.Disable();
            animator.SetFloat("speedPercent", 0f);
        }
    }

    void FixedUpdate()
    {
        // 1) Read raw input
        moveInput = moveAction.action.ReadValue<Vector2>();

        // 2) Dead‑zone check
        if (moveInput.sqrMagnitude <= movementDeadzone * movementDeadzone)
        {
            animator.SetFloat("speedPercent", 0f);
            return;
        }

        // 3) Build camera‑relative move direction
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();

        // 4) Smoothly rotate to face that exact direction
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );

        // 5) Move in world‑space along that same vector
        transform.Translate(moveDir * walkSpeed * Time.deltaTime, Space.World);

        // 6) Drive the animator
        float speedPercent = Mathf.Clamp01(moveInput.magnitude);
        animator.SetFloat("speedPercent", speedPercent);
    }
}
