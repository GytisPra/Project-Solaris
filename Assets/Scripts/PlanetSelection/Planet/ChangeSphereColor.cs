using UnityEngine;

public class ChangeSphereColor : MonoBehaviour
{
    public Color sphereColor;

    void Start()
    {
        GetComponent<Renderer>().material.color = sphereColor;
    }
}
