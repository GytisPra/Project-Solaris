using UnityEngine;

public class RefractionCalculator
{
    public static bool TryCalculateRefraction(
        Vector3 incidentDir, Vector3 normal, float n1, float n2,
        out Vector3 refractedDir, out float angleOfIncidence, out float angleOfRefraction)
    {
        float cosThetaI = Vector3.Dot(-incidentDir.normalized, normal);
        angleOfIncidence = Mathf.Abs(Mathf.Acos(Mathf.Clamp(cosThetaI, -1f, 1f)) * Mathf.Rad2Deg - 180f);

        float sinThetaI = Mathf.Sqrt(1f - cosThetaI * cosThetaI);
        float sinThetaT = (n1 / n2) * sinThetaI;

        if (sinThetaT > 1f)
        {
            refractedDir = Vector3.Reflect(incidentDir, normal);
            angleOfIncidence = 0f;
            angleOfRefraction = 0f; 
            return false;
        }

        float cosThetaT = Mathf.Sqrt(1f - sinThetaT * sinThetaT);
        angleOfRefraction = Mathf.Asin(Mathf.Clamp(sinThetaT, -1f, 1f)) * Mathf.Rad2Deg;

        refractedDir = (n1 / n2) * (incidentDir - normal * cosThetaI) + normal * cosThetaT;
        refractedDir.Normalize();

        return true;
    }
}