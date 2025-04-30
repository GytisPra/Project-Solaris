using UnityEngine;

public class RotatePortal : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        myTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
