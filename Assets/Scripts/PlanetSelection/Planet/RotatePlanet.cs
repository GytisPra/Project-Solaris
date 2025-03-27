using System;
using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    public float rotationSpeed;

    private Vector3 rotationAxis;

    void Start()
    {
        if (Math.Abs(transform.rotation.x) > 0) {
            rotationAxis = Vector3.up;
        } else {
            rotationAxis = transform.up;
        }
    }

    void Update()
    {
        transform.Rotate(rotationAxis, -rotationSpeed * Time.deltaTime, Space.World);
    }
}
