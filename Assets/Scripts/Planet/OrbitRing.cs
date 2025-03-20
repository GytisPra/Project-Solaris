using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class OrbitRing : MonoBehaviour
{
    
    public float trailTime = 5f;
    public float trailWidth = 0.05f;
    public float minWidth = 0.05f;
    public float maxWidth = 0.1f;
    public Gradient trailColorGradient;

    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0;
        trail.colorGradient = trailColorGradient;
    }
}
