using System.Collections;
using UnityEngine;

public class AnimateLampRay : MonoBehaviour
{
    public float adjustmentSpeed = 2f;
    public float rayLengthToScaleRatio = 5f / 1.4f;

    [SerializeField] private Camera fromCamera;
    [SerializeField] private Camera toCamera;
    [SerializeField] private AnimateLensRay animateLensRay;
    [SerializeField] private IceMelt iceMelt;
    [SerializeField] private Transform lampRayTransform;
    [SerializeField] private Transform lensRayTransform;
    [SerializeField] private Transform lensTransform;
    [SerializeField] private ExperimentResultsUI experimentResultsUI;
    private Transform lampTransform;
    private bool isDistanceCorrect = false;

    void Start()
    {
        lampTransform = transform;
    }

    public IEnumerator EnableLamp()
    {
        Debug.Log("Turning on lamp!");

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

        Debug.Log("Lamp has finished its animation!");

        if (iceMelt.IsMelted())
        {
            Debug.Log("Starting to fully reveal lens ray!");

            yield return StartCoroutine(animateLensRay.ReavealFully()); // Wait for lensRay to be fully reveled

            Debug.Log("Lens ray finished fully revealing!");
        }
        else
        {
            Debug.Log("Starting to reveal lens ray!");

            yield return StartCoroutine(animateLensRay.Reveal()); // Wait for the lens ray to be revealed

            Debug.Log("Lens ray finished revealing!");
            if (iceMelt != null && isDistanceCorrect)
            {
                Debug.Log("Distance correct starting to melt ice!");

                yield return StartCoroutine(iceMelt.Melt()); // Start melting the ice and wait for it to finish

                Debug.Log("Ice is done melting!");
            }
        }

        Debug.Log("Starting to reveal the results!");

        yield return StartCoroutine(experimentResultsUI.DisplayResult(isDistanceCorrect)); // wait for the results to be displayed

        Debug.Log("Results have been shown!\nWaiting 200ms then begining transition back!");

        yield return new WaitForSeconds(0.2f); // Wait a little bit (200ms)

        yield return StartCoroutine(CameraTransition.Instance.TransitionBack());

        Debug.Log("Finished transition back!");
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
