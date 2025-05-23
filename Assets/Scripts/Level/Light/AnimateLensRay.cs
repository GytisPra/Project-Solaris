using System.Collections;
using UnityEngine;

public class AnimateLensRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;

    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private IceMelt iceMelt;
    [SerializeField] private float dioptreOfLens = 2f;
    [SerializeField] private float blenderZLength = 4.5f;

    private float targetRevealHeight = -22.31f;
    private float startingRevealHeight = -17.3f;
    private float fullRevealHeight = -23f;

    private MeshRenderer lensRayRenderer;
    private float currentRevealHeight;
    private float focalLength;

    private void Start()
    {
        startingRevealHeight = transform.position.z;
        lensRayRenderer = lensRayTransform.GetComponent<MeshRenderer>();
        lensRayRenderer.material.SetFloat("_RevealHeight", startingRevealHeight);
        currentRevealHeight = startingRevealHeight;
        targetRevealHeight = iceMelt.transform.position.z + 0.5f; // Add 0.5 because the ice Z is in the middle of it we want the edge
    }

    private void Update()
    {
        if (Mathf.Approximately(dioptreOfLens, 0f))
            return;

        focalLength = 1f / dioptreOfLens; // meters
        float zScale = focalLength / blenderZLength;

        Vector3 scale = lensRayTransform.localScale;
        scale.z = Mathf.Round(zScale * 100f) / 100f;
        lensRayTransform.localScale = scale;
    }

   

    public IEnumerator ReavealFully()
    {
        currentRevealHeight = transform.position.z;
        fullRevealHeight = currentRevealHeight - focalLength;
        lensRayRenderer.material.SetFloat("_RevealHeight", currentRevealHeight);
        yield return StartCoroutine(AnimateRevealHeight(currentRevealHeight, fullRevealHeight));
    }

    public void ReavealFullyFromCurrentRevealHeight()
    {
        fullRevealHeight = currentRevealHeight - focalLength;
        StartCoroutine(AnimateRevealHeight(currentRevealHeight, fullRevealHeight));
    }

    public IEnumerator Reveal()
    {
        currentRevealHeight = transform.position.z;

        // Check if the light ray reaches the ice
        if (transform.position.z - 4.5f < targetRevealHeight) 
        {
            lensRayRenderer.material.SetFloat("_RevealHeight", currentRevealHeight);
            yield return StartCoroutine(AnimateRevealHeight(currentRevealHeight, targetRevealHeight));
        }
        else // If it does not reach then just fully reveal it
        {
            yield return StartCoroutine(ReavealFully());
        }
    }

    private IEnumerator AnimateRevealHeight(float fromHeight, float toHeight)
    {
        float elapsedTime = 0f;
        float duration = Mathf.Abs(toHeight - fromHeight) / adjustmentSpeed;

        currentRevealHeight = fromHeight;

        while (elapsedTime < duration)
        {
            currentRevealHeight = Mathf.Lerp(fromHeight, toHeight, elapsedTime / duration);

            lensRayRenderer.material.SetFloat("_RevealHeight", currentRevealHeight);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        lensRayRenderer.material.SetFloat("_RevealHeight", toHeight);
        currentRevealHeight = toHeight;
    }

    public void ResetRevealHeight()
    {
        lensRayRenderer.material.SetFloat("_RevealHeight", startingRevealHeight);
        currentRevealHeight = startingRevealHeight;
    }
}
