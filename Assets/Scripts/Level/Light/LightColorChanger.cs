using UnityEngine;

public class LightColorChanger : MonoBehaviour
{
    public float wavelength;
    private Material lampRayMaterial;
    private Material lensRayMaterial;

    [SerializeField] private Transform lampRay;
    [SerializeField] private Transform lensRay;

    private void Start()
    {
        lampRayMaterial = lampRay.GetComponent<Renderer>().material;
        lensRayMaterial = lensRay.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        lampRayMaterial.SetColor("_EmissionColor", WavelengthToColor(wavelength));
        lensRayMaterial.SetColor("_EmissionColor", WavelengthToColor(wavelength));
    }

    public static Color WavelengthToColor(float wavelengthNm)
    {
        float gamma = 0.8f;
        float red = 0f, green = 0f, blue = 0f;
        float factor;

        if (wavelengthNm >= 380f && wavelengthNm <= 440f)
        {
            red = -(wavelengthNm - 440f) / (440f - 380f);
            blue = 1.0f;
        }
        else if (wavelengthNm > 440f && wavelengthNm <= 490f)
        {
            green = (wavelengthNm - 440f) / (490f - 440f);
            blue = 1.0f;
        }
        else if (wavelengthNm > 490f && wavelengthNm <= 510f)
        {
            green = 1.0f;
            blue = -(wavelengthNm - 510f) / (510f - 490f);
        }
        else if (wavelengthNm > 510f && wavelengthNm <= 580f)
        {
            red = (wavelengthNm - 510f) / (580f - 510f);
            green = 1.0f;
        }
        else if (wavelengthNm > 580f && wavelengthNm <= 645f)
        {
            red = 1.0f;
            green = -(wavelengthNm - 645f) / (645f - 580f);
        }
        else if (wavelengthNm > 645f && wavelengthNm <= 780f)
        {
            red = 1.0f;
        }

        // Intensity adjustment
        if (wavelengthNm >= 380f && wavelengthNm <= 420f)
        {
            factor = 0.3f + 0.7f * (wavelengthNm - 380f) / (420f - 380f);
        }
        else if (wavelengthNm > 420f && wavelengthNm <= 700f)
        {
            factor = 1.0f;
        }
        else if (wavelengthNm > 700f && wavelengthNm <= 780f)
        {
            factor = 0.3f + 0.7f * (780f - wavelengthNm) / (780f - 700f);
        }
        else
        {
            factor = 0f;
        }

        red = Mathf.Clamp01(Mathf.Pow(red * factor, gamma));
        green = Mathf.Clamp01(Mathf.Pow(green * factor, gamma));
        blue = Mathf.Clamp01(Mathf.Pow(blue * factor, gamma));

        return new Color(red, green, blue, 1f); // Alpha = 1 (fully opaque)
    }
}

