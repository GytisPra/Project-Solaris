using System.Collections;
using UnityEngine;

public class AnimateLampRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;
    public float rayLengthToScaleRatio = 5f / 1.4f;

    [SerializeField] private AnimateLensRay animateLensRay;
    [SerializeField] private IceMelt iceMelt;
    [SerializeField] private Transform lampRayTransform;
    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private Transform lensTransform;
    private Transform lampTransform;
    private bool isOn = false;
    private bool isDistanceCorrect = false;

    void Start()
    {
        lampTransform = transform;
    }

    void Update()
    {
        if (isOn)
        {
            float distance = Vector3.Distance(lampTransform.position, lensTransform.position);
            float adjustedScaleZ = distance / rayLengthToScaleRatio;

            Vector3 targetScale = new(1, 1, adjustedScaleZ);
            lampRayTransform.localScale = Vector3.MoveTowards(
                lampRayTransform.localScale,
                targetScale,
                adjustmentSpeed * Time.deltaTime
            );

            if (lampRayTransform.localScale == targetScale)
            {
                lensRayTransform.gameObject.SetActive(true);
                if (iceMelt.IsDoneMelting())
                {
                    animateLensRay.ReavealFully();
                } else
                {
                    StartCoroutine(HandleLensRayReveal());
                }
            }
        }
        else
        {
            animateLensRay.ResetRevealHeight();
            lensRayTransform.gameObject.SetActive(false);

            Vector3 targetScale = new(1, 1, 0);
            lampRayTransform.localScale = Vector3.MoveTowards(
                lampRayTransform.localScale,
                targetScale,
                adjustmentSpeed * Time.deltaTime
            );
        }
        
    }

    private IEnumerator HandleLensRayReveal()
    {
        yield return StartCoroutine(animateLensRay.Reveal());

        if (iceMelt != null && isDistanceCorrect)
        {
            iceMelt.Melt();
        }
    }

    public void EnableLamp() => isOn = true;
    public void DisableLamp() => isOn = false;

    public void DistanceCorrect() => isDistanceCorrect = true;
}
