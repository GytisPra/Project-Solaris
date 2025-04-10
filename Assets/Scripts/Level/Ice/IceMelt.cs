using System.Collections;
using UnityEngine;

public class IceMelt : MonoBehaviour
{
    public float meltSpeed = 2f;

    private Transform iceTransform;
    [SerializeField] AnimateLensRay animateLensRay;
    private bool targetReached = false;

    void Start()
    {
        iceTransform = transform;
    }

    public IEnumerator Melt()
    {
        Vector3 targetScale = new(0, 0, 0);

        while (Vector3.Distance(iceTransform.localScale, targetScale) > 0.001f)
        {
            iceTransform.localScale = Vector3.MoveTowards(
                iceTransform.localScale,
                targetScale,
                meltSpeed * Time.deltaTime
            );

            yield return null;
        }

        iceTransform.localScale = targetScale;

        foreach (BoxCollider collider in iceTransform.GetComponents<BoxCollider>())
        {
            collider.enabled = false;
        }

        animateLensRay.ReavealFullyFromCurrentRevealHeight();

        targetReached = true;
    }
    public bool IsDoneMelting() => targetReached;
}
