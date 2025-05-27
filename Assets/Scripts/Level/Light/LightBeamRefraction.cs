using UnityEngine;

public class LightBeamRefraction : MonoBehaviour
{
    [SerializeField] private Transform rayAir;
    [SerializeField] private Transform rayWater;
    [SerializeField] private Transform waterSurface;
    [SerializeField] private Vector3 rotationOffset = new(0f, -180f, 0f);

    [Header("Refraction Settings")]
    public float airRefractiveIndex = 1.0f;
    public float waterRefractiveIndex = 1.33f;
    public float maxWaterBeamScale = 0.4f;
    public float minWaterBeamScale = 0.3f;

    private void Update()
    {
        ApplyRefraction();
    }

    private void ApplyRefraction()
    {
        Vector3 incidentDir = rayAir.right;
        Vector3 normal = Vector3.up;

        Vector3 incidentOrigin = rayAir.position;

        // Calculate t where the ray intersects the water surface
        float t = (waterSurface.position.y - incidentOrigin.y) / incidentDir.y;
        Vector3 intersectionPoint = incidentOrigin + incidentDir * t;

        // Move the water beam to this intersection point
        float beamHeight = rayWater.localScale.x * 2f;
        rayWater.position = intersectionPoint + rayWater.right * beamHeight;

        if (!RefractionCalculator.TryCalculateRefraction(
            incidentDir, normal,
            airRefractiveIndex, waterRefractiveIndex, 
            out var refractedDir, out var scaleFactor, 
            minWaterBeamScale, maxWaterBeamScale))
        {
            Vector3 reflectedDir = Vector3.Reflect(incidentDir, normal).normalized;
            Debug.DrawRay(intersectionPoint, reflectedDir * 2f, Color.magenta);

            rayWater.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, reflectedDir)) * Quaternion.Euler(rotationOffset);
            return;
        }

        refractedDir.Normalize();

        Vector3 newScale = rayAir.localScale;
        newScale.y = scaleFactor;
        newScale.z = scaleFactor;
        rayWater.localScale = newScale;

        rayWater.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, refractedDir)) * Quaternion.Euler(rotationOffset);
    }
}
