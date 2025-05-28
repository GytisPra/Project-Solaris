using TMPro;
using UnityEngine;

public class LightBeamRefraction : MonoBehaviour
{
    [SerializeField] private Transform rayAir;
    [SerializeField] private Transform rayWater;
    [SerializeField] private Transform waterSurface;
    [SerializeField] private Vector3 rotationOffset = new(0f, -180f, 0f);
    [SerializeField] private TMP_Text angleOfIncidenceText;
    [SerializeField] private TMP_Text angleOfRefractionText;
    [SerializeField] private AnimationCurve scaleByAngle;
    private Vector3 intersectionPoint;
    private Vector3 incidentDir;

    [Header("Refraction Settings")]
    public float airRefractiveIndex = 1.0f;
    public float waterRefractiveIndex = 1.33f;

    private void Update()
    {
        ApplyRefraction();
        Vector3 incidentOrigin = rayAir.position;

        incidentDir = rayAir.right;
        float t = (waterSurface.position.y - incidentOrigin.y) / incidentDir.y;
        intersectionPoint = incidentOrigin + incidentDir * t;

        // Move the water beam to this intersection point
        float beamHeight = rayWater.localScale.x * 2f;
        rayWater.position = intersectionPoint + rayWater.right * beamHeight;
    }

    private void ApplyRefraction()
    {
        Vector3 normal = Vector3.up;

        if (!RefractionCalculator.TryCalculateRefraction(
            incidentDir, normal,
            airRefractiveIndex, waterRefractiveIndex, 
            out var refractedDir, out var angleOfIncidence, out var angleOfRefraction))
        {
            Vector3 reflectedDir = Vector3.Reflect(incidentDir, normal).normalized;
            Debug.DrawRay(intersectionPoint, reflectedDir * 2f, Color.magenta);

            rayWater.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, reflectedDir)) * Quaternion.Euler(rotationOffset);
            return;
        }

        angleOfIncidenceText.text = $"{angleOfIncidence:F2}°";
        angleOfRefractionText.text = $"{angleOfRefraction:F2}°";

        refractedDir.Normalize();

        Debug.DrawRay(rayAir.position, -rayAir.right * 2f, Color.red);
        Debug.DrawRay(intersectionPoint, rayWater.right * 2f, Color.cyan);
        Debug.DrawRay(new(intersectionPoint.x, intersectionPoint.y - 1f, intersectionPoint.z), normal * 2f, Color.green);

        float scaleFactor = scaleByAngle.Evaluate(angleOfIncidence);

        Vector3 newScale = rayAir.localScale;
        newScale.y = scaleFactor;
        rayWater.localScale = newScale;

        rayWater.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, refractedDir)) * Quaternion.Euler(rotationOffset);
    }
}
