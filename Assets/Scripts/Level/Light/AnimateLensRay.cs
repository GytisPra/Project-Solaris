using System.Collections;
using UnityEngine;

public class AnimateLensRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;

    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private IceMelt iceMelt;
    [SerializeField] private float dioptreOfLens = 2f;
    [SerializeField] private float rayOriginalLength = 1f;
    [SerializeField] private float blenderZLength = 4.5f;

    private readonly float targetRevealHeight = -22.31f;
    private float startingRevealHeight = -17.3f;
    private float fullRevealHeight = -23f;

    private MeshRenderer lensRayRenderer;
    private float currentRevealHeight;

    private void Start()
    {
        startingRevealHeight = transform.parent.position.z;
        lensRayRenderer = lensRayTransform.GetComponent<MeshRenderer>();
        lensRayRenderer.material.SetFloat("_RevealHeight", startingRevealHeight);
        currentRevealHeight = startingRevealHeight;
    }

    private void Update()
    {
        if (Mathf.Approximately(dioptreOfLens, 0f))
            return;

        float focalLength = 1f / dioptreOfLens; // meters
        float zScale = focalLength / blenderZLength;

        Vector3 scale = lensRayTransform.localScale;
        scale.z = Mathf.Round(zScale * 100f) / 100f;
        lensRayTransform.localScale = scale;
    }

    public IEnumerator ReavealFully()
    {
        currentRevealHeight = transform.parent.position.z;
        fullRevealHeight = currentRevealHeight - 4.5f; // 4.5 is the length of the ray
        lensRayRenderer.material.SetFloat("_RevealHeight", currentRevealHeight);
        yield return StartCoroutine(AnimateRevealHeight(currentRevealHeight, fullRevealHeight));
    }

    public void ReavealFullyFromCurrentRevealHeight()
    {
        StartCoroutine(AnimateRevealHeight(currentRevealHeight, fullRevealHeight));
    }

    public IEnumerator Reveal()
    {
        currentRevealHeight = transform.parent.position.z;
        lensRayRenderer.material.SetFloat("_RevealHeight", currentRevealHeight);
        yield return StartCoroutine(AnimateRevealHeight(currentRevealHeight, targetRevealHeight));
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
