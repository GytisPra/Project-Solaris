using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    private Vector3 initialPosition;
    public float fallThreshold = -10f;
    public float gravityMultiplier = 2f;

    void Start()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        transform.SetPositionAndRotation(initialPosition, Quaternion.identity);
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
