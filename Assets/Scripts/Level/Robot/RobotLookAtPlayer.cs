using UnityEngine;

public class RobotLookAtPlayer : MonoBehaviour
{
    public Transform headBone;
    public Transform target;
    public float headRotationSpeed = 5f;
    public float maxHeadAngle = 90f;

    private bool isLooking;
    private float currentLookWeight;
    private Quaternion initialRotation;

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
        }
        else if (newState == GameState.Gameplay)
        {
            isLooking = false;
        }
    }

    private void Start()
    {
        initialRotation = headBone.rotation;
    }

    void Update()
    {
        float targetWeight = isLooking ? 1f : 0f;
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * headRotationSpeed);
    }

    void LateUpdate()
    {
        if (headBone == null) return;

        Quaternion targetRotation;

        if (isLooking && target != null)
        {
            Vector3 directionToTarget = (target.position - headBone.position).normalized;
            targetRotation = Quaternion.LookRotation(directionToTarget);

            float angle = Quaternion.Angle(headBone.rotation, targetRotation);
            if (angle > maxHeadAngle)
            {
                targetRotation = Quaternion.Slerp(headBone.rotation, targetRotation, maxHeadAngle / angle);
            }
        }
        else
        {
            targetRotation = initialRotation;
        }

        headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, Time.deltaTime * headRotationSpeed * currentLookWeight);
    }
}
