using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class OrbitRing : MonoBehaviour
{
    
    public float trailTime = 5f;
    public float trailWidth = 0.05f;
    public Gradient trailColorGradient;

    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0; // Taper to 0 for a smooth fade
        trail.colorGradient = trailColorGradient; // Use a gradient to fade the trail
    }
}
