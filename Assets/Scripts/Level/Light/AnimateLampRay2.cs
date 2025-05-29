using System.Collections;
using UnityEngine;

public class AnimateLampRay2 : MonoBehaviour
{
    public float adjustmentSpeed = 2f;
    [Range(380f, 750f)]
    public float wavelength = 550f;
    public float colorChangeSpeed = 2f;

    [SerializeField] private Transform lampRayTransform;

    private Material rayMaterial;
    private bool isEnabled;

    void Start()
    {
        rayMaterial = lampRayTransform.GetComponent<Renderer>().material;

        //StartCoroutine(ChangeColor(wavelength));
        //StartCoroutine(EnableLamp());
    }

    public IEnumerator ChangeColor(float newWavelength)
    {
        wavelength = newWavelength;

        Color targetColor = WavelengthToUnityColor.WavelengthToColor(newWavelength);
        Color currentColor = rayMaterial.GetColor("_EmissionColor");

        while (Vector4.Distance(currentColor, targetColor) > 0.01f)
        {
            currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);
            rayMaterial.SetColor("_EmissionColor", currentColor);

            yield return null;
        }

        rayMaterial.SetColor("_EmissionColor", targetColor);
    }

    public IEnumerator EnableLamp()
    {
        if (isEnabled)
        {
            yield break;
        }

        isEnabled = true;

        Vector3 targetScale = new(1, 1, 1);

        while (Vector3.Distance(lampRayTransform.localScale, targetScale) > 0.01f)
        {
            lampRayTransform.localScale = Vector3.MoveTowards(
                lampRayTransform.localScale,
                targetScale,
                adjustmentSpeed * Time.deltaTime
            );

            yield return null;
        }

        lampRayTransform.localScale = targetScale;
    }

    public IEnumerator DisableLamp()
    {
        if (!isEnabled)
        {
            yield break;
        }

        isEnabled = false;

        Vector3 targetScale = new(0, 1, 1);

        while (Vector3.Distance(lampRayTransform.localScale, targetScale) > 0.01f)
        {
            lampRayTransform.localScale = Vector3.MoveTowards(
                lampRayTransform.localScale,
                targetScale,
                adjustmentSpeed * Time.deltaTime
            );

            yield return null;
        }

        lampRayTransform.localScale = targetScale;
    }
}
