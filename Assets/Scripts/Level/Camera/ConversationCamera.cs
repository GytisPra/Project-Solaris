using UnityEngine;

public class ConversationCamera : MonoBehaviour
{
    public Transform follow;
    public Transform target;
    public Camera conversationCamera;

    [SerializeField] private Vector3 offset = new(0.5f, 2f, -4f);

    public void PrepareForConversation()
    {
        if (target != null && follow != null && conversationCamera.isActiveAndEnabled)
        {
            // Make offset relative to the follow's rotation (local space to world space)
            Vector3 shoulderOffset = follow.TransformDirection(offset);
            Vector3 desiredPosition = follow.position + shoulderOffset;

            Vector3 directionToTarget = target.position - desiredPosition;
            directionToTarget.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            conversationCamera.transform.SetPositionAndRotation(desiredPosition, targetRotation);
        }
    }
}
