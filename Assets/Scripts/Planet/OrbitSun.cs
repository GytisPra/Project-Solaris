using UnityEngine;

public class OrbitSun : MonoBehaviour
{
    public GameObject sun;
    public float orbitSpeed;

    private float orbitRadius;
    private float angle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set orbitRadius based on the initial z-coordinate of the object relative to the sun
        orbitRadius = Mathf.Abs(transform.position.z - sun.transform.position.z);
        
        // Calculate the initial angle based on the starting position
        Vector3 direction = transform.position - sun.transform.position;
        angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
    }

    // Update is called once per frame
    void Update()
    {
        // Increase angle over time based on orbitSpeed
        angle += orbitSpeed * Time.deltaTime;
        float radian = angle * Mathf.Deg2Rad;

        // Calculate new position
        float x = sun.transform.position.x + Mathf.Cos(radian) * orbitRadius;
        float z = sun.transform.position.z + Mathf.Sin(radian) * orbitRadius;

        transform.position = new Vector3(x, transform.position.y, z);
    }
}
