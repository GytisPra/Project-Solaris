using System.Collections;
using UnityEngine;

public class AnimateLampRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;
    public float rayLengthToScaleRatio = 5f / 1.4f;

    [SerializeField] private Camera fromCameraPotrait;
    [SerializeField] private Camera fromCamera;
    [SerializeField] private Camera toCamera;
    [SerializeField] private AnimateLensRay animateLensRay;
    [SerializeField] private IceMelt iceMelt;
    [SerializeField] private Transform lampRayTransform;
    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private Transform lensTransform;
    private Transform lampTransform;
    private bool isDistanceCorrect = false;

    void Start()
    {
        lampTransform = transform;
    }

    public IEnumerator EnableLamp()
    {
        float distance = Vector3.Distance(lampTransform.position, lensTransform.position);
        float adjustedScaleZ = distance / rayLengthToScaleRatio;

        Vector3 targetScale = new(1, 1, adjustedScaleZ);

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
        lensRayTransform.gameObject.SetActive(true);

        if (iceMelt.IsDoneMelting())
        {
            yield return StartCoroutine(animateLensRay.ReavealFully()); // Wait for lensRay to be fully reveled
        }
        else
        {
            yield return StartCoroutine(animateLensRay.Reveal()); // Wait for the lens ray to be revealed
            if (iceMelt != null && isDistanceCorrect)
            {
                yield return StartCoroutine(iceMelt.Melt()); // Start melting the ice and wait for it to finish
            }
        }

        yield return new WaitForSeconds(0.2f); // Wait a little bit (200ms)

        Camera fromCamera = Screen.orientation == ScreenOrientation.Portrait ? fromCameraPotrait : this.fromCamera;
        StartCoroutine(CameraTransition.Instance.SmoothCameraTransition(fromCamera, toCamera, false));
    }

    public IEnumerator DisableLamp()
    {
        Vector3 targetScale = new(1, 1, 0);

        animateLensRay.ResetRevealHeight();
        lensRayTransform.gameObject.SetActive(false);

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

    public void DistanceCorrect() => isDistanceCorrect = true;
}
