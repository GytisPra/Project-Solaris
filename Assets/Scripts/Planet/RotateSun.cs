using UnityEngine;

public class RotateSun : MonoBehaviour
{
    public float rotationSpeed;

    void Update()
    {
        float radian = rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
        transform.Rotate(0, -radian, 0);
    }
}
