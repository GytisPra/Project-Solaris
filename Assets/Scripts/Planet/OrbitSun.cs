using UnityEngine;

public class OrbitSun : MonoBehaviour
{
    public GameObject sun;
    public float orbitSpeed;

    private float orbitRadius;
    private float angle;

    void Start()
    {
        orbitRadius = Mathf.Abs(transform.position.z - sun.transform.position.z);
        
        Vector3 direction = transform.position - sun.transform.position;
        angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }

    void FixedUpdate()
    {
        angle += orbitSpeed * Time.deltaTime;
        float radian = angle * Mathf.Deg2Rad;

        float x = sun.transform.position.x + Mathf.Cos(radian) * orbitRadius;
        float z = sun.transform.position.z + Mathf.Sin(radian) * orbitRadius;

        transform.position = new Vector3(x, transform.position.y, z);
    }
}
