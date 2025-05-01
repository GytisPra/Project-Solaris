using UnityEngine;

public class CharLookController : MonoBehaviour
{
    public Transform target;
    public float bodyRotationSpeed = 5f;
    public float headRotationSpeed = 5f;
    public bool finishedRotation;

    private Animator animator;
    private bool isLooking;
    private float currentLookWeight = 0f;
    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Conversation)
        {
            isLooking = true;
            finishedRotation = false;
        }
        else if (newState == GameState.Gameplay)
        {
            isLooking = false;
            finishedRotation = false;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float targetWeight = isLooking ? 1f : 0f;
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * headRotationSpeed);

        if (isLooking && target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * bodyRotationSpeed);
            }


            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget); // Angle between forward and target
            if (currentLookWeight >= 0.8f && angleToTarget < 12f) // 15 degree tolerance
            {
                finishedRotation = true;
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        Vector3 lookPosition = target != null
            ? target.position
            : transform.position + transform.forward * 10f;

        float angleToTarget = Vector3.Angle(transform.forward, (lookPosition - transform.position).normalized);
        float dynamicBodyWeight = Mathf.InverseLerp(30f, 90f, angleToTarget);
        dynamicBodyWeight = Mathf.Clamp(dynamicBodyWeight, 0f, 0.6f);

        animator.SetLookAtWeight(currentLookWeight, dynamicBodyWeight, 0.8f, 0.5f, 0.5f);
        animator.SetLookAtPosition(lookPosition);
    }
}