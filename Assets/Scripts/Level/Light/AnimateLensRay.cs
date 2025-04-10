using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLensRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;

    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private IceMelt iceMelt;

    private readonly float targetRevealHeight = -22.31f;
    private readonly float startingRevealHeight = -17.3f;
    private readonly float fullRevealHeight = -30f;

    private MeshRenderer lensRayRenderer;
    private float currentRevealHeight;

    private void Start()
    {
        lensRayRenderer = lensRayTransform.GetComponent<MeshRenderer>();
        lensRayRenderer.material.SetFloat("_RevealHeight", startingRevealHeight);
        currentRevealHeight = startingRevealHeight;
    }

    public void ReavealFully()
    {
        StartCoroutine(AnimateRevealHeight(currentRevealHeight, fullRevealHeight));
    }

    public IEnumerator Reveal()
    {
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
