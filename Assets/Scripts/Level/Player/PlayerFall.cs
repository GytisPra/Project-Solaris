using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Transform platform;
    public float fallThreshold = -10f;
    public float gravityMultiplier = 9.81f;

    void Start()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().useGravity = true;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (transform.localPosition.y < fallThreshold)
        {
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
