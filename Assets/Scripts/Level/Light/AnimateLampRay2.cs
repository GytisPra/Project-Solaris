using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AnimateLampRay2 : MonoBehaviour
{
    public float adjustmentSpeed = 2f;
    [Range(380f, 750f)]
    public float wavelength = 550f;

    [SerializeField] private Transform lampRayTransform;

    private Material rayMaterial;

    void Start()
    {
        rayMaterial = lampRayTransform.GetComponent<Renderer>().material;

        StartCoroutine(EnableLamp());
    }

    private void Update()
    {
        rayMaterial.SetColor("_EmissionColor", WavelengthToUnityColor.WavelengthToColor(wavelength));
    }

    public IEnumerator EnableLamp()
    {
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
