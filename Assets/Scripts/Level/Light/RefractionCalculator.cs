using UnityEngine;

public class RefractionCalculator
{
    public static bool TryCalculateRefraction(
        Vector3 incidentDir, Vector3 normal, float n1, float n2,
        out Vector3 refractedDir, out float scaleFactor, float minScale, float maxScale)
    {
        float cosThetaI = Vector3.Dot(-incidentDir.normalized, normal);
        float sinThetaI = Mathf.Sqrt(1f - cosThetaI * cosThetaI);
        float sinThetaT = (n1 / n2) * sinThetaI;

        if (sinThetaT > 1f)
        {
            refractedDir = Vector3.Reflect(incidentDir, normal);
            scaleFactor = 0f;
            return false;
        }

        float cosThetaT = Mathf.Sqrt(1f - sinThetaT * sinThetaT);
        refractedDir = (n1 / n2) * (incidentDir - normal * cosThetaI) + normal * cosThetaT;
        refractedDir.Normalize();

        scaleFactor = Mathf.Lerp(minScale, maxScale, sinThetaI);
        return true;
    }
}