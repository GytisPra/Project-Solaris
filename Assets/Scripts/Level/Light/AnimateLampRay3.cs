using System.Collections;
using UnityEngine;

public class AnimateLampRay3 : MonoBehaviour
{
    [System.Serializable]
    public struct Surfaces
    {
        public Transform leftWall;
        public Transform rightWall;
        public Transform bottom;
        public Transform waterSurface;
    }

    [SerializeField] private Renderer waterRayRenderer;
    [SerializeField] private Renderer airRayRenderer;
    [SerializeField] private Surfaces surfaces;

    private Material airRayMaterial;
    private Material waterRayMaterial;

    public float animationSpeed = 2f;

    private void Start()
    {
        airRayMaterial = airRayRenderer.material;
        waterRayMaterial = waterRayRenderer.material;

        float left = surfaces.rightWall.position.x;
        float right = surfaces.leftWall.position.x;
        float waterY = surfaces.waterSurface.position.y;

        waterRayMaterial.SetFloat("_RevealHeightLeft", left);
        waterRayMaterial.SetFloat("_RevealHeightRight", right);
        waterRayMaterial.SetFloat("_RevealHeightUp", waterY);
        waterRayMaterial.SetFloat("_RevealHeightDown", waterY);

        airRayMaterial.SetFloat("_RevealHeightDown", transform.position.y);
        airRayMaterial.SetFloat("_RevealHeightUp", transform.position.y);
        airRayMaterial.SetFloat("_RevealHeightLeft", left);
        airRayMaterial.SetFloat("_RevealHeightRight", right);
    }

    public IEnumerator RevealRays()
    {
        yield return StartCoroutine(RevealAirBeam());
        yield return StartCoroutine(RevealWaterBeam());
    }

    public IEnumerator HideRays()
    {
        yield return StartCoroutine(HideAirBeam());
        yield return StartCoroutine(HideWaterBeam());
    }

    private IEnumerator RevealAirBeam()
    {
        float targetDown = surfaces.waterSurface.position.y - 0.001f;
        float left = surfaces.rightWall.position.x;
        float right = surfaces.leftWall.position.x;

        airRayMaterial.SetFloat("_RevealHeightDown", transform.position.y);
        airRayMaterial.SetFloat("_RevealHeightUp", transform.position.y);
        airRayMaterial.SetFloat("_RevealHeightLeft", left);
        airRayMaterial.SetFloat("_RevealHeightRight", right);

        yield return StartCoroutine(AnimateFloat(airRayMaterial, "_RevealHeightDown", airRayMaterial.GetFloat("_RevealHeightDown"), targetDown));
    }

    private IEnumerator RevealWaterBeam()
    {
        float down = surfaces.bottom.position.y;
        float left = surfaces.rightWall.position.x;
        float right = surfaces.leftWall.position.x;
        float waterY = surfaces.waterSurface.position.y;

        waterRayMaterial.SetFloat("_RevealHeightLeft", left);
        waterRayMaterial.SetFloat("_RevealHeightRight", right);
        waterRayMaterial.SetFloat("_RevealHeightUp", waterY);
        waterRayMaterial.SetFloat("_RevealHeightDown", waterY);

        yield return StartCoroutine(AnimateFloat(waterRayMaterial, "_RevealHeightDown", waterRayMaterial.GetFloat("_RevealHeightDown"), down));
    }

    private IEnumerator HideAirBeam()
    {
        float initialUp = airRayMaterial.GetFloat("_RevealHeightUp");
        float targetDown = surfaces.waterSurface.position.y;

        yield return StartCoroutine(AnimateFloat(airRayMaterial, "_RevealHeightUp", initialUp, targetDown));
    }

    private IEnumerator HideWaterBeam()
    {
        float initialUp = waterRayMaterial.GetFloat("_RevealHeightUp");
        float targetUp = surfaces.bottom.position.y;

        yield return StartCoroutine(AnimateFloat(waterRayMaterial, "_RevealHeightUp", initialUp, targetUp));

        waterRayMaterial.SetFloat("_RevealHeightLeft", 0);
        waterRayMaterial.SetFloat("_RevealHeightRight", 0);
    }

    private IEnumerator AnimateFloat(Material mat, string property, float from, float to, float epsilon = 0.001f)
    {
        float value = from;
        while (Mathf.Abs(value - to) > epsilon)
        {
            value = Mathf.MoveTowards(value, to, animationSpeed * Time.deltaTime);
            mat.SetFloat(property, value);
            yield return null;
        }

        mat.SetFloat(property, to); // ensure exact
    }
}
