using UnityEngine;

public class IceMelt : MonoBehaviour
{
    public float meltSpeed = 2f;

    private Transform iceTransform;
    [SerializeField] AnimateLensRay animateLensRay;
    private bool isMelting = false;
    private bool targetReached = false;

    void Start()
    {
        iceTransform = transform;
    }

    void Update()
    {
        if (isMelting && !targetReached)
        {
            Vector3 targetScale = new(0, 0, 0);
            iceTransform.localScale = Vector3.MoveTowards(
                iceTransform.localScale,
                targetScale,
                meltSpeed * Time.deltaTime
                );

            if (Vector3.Distance(iceTransform.localScale, targetScale) < 0.001f)
            {
                foreach (BoxCollider collider in iceTransform.GetComponents<BoxCollider>())
                {
                    collider.enabled = false;
                }

                animateLensRay.ReavealFully();

                targetReached = true;
            }
        }
    }

    public void Melt() => isMelting = true;
    public void StopMelt() => isMelting = false;
    public bool IsDoneMelting() => targetReached;
}
