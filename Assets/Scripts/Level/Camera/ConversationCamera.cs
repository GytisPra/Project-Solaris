using UnityEngine;

public class ConversationCamera : MonoBehaviour
{
    public Vector3 postionOffset = new(1f, 2.5f, 1f);
    public Transform target;
    private Transform myTransform;
    private Transform parent;
   
    void Start()
    {
        myTransform = transform;
        parent = transform.parent;
    }

    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - myTransform.position);

        Vector3 targetPosition = parent.position + postionOffset;
        myTransform.SetPositionAndRotation(targetPosition, targetRotation);
    }
}
