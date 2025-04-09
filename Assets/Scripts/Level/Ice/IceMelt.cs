using UnityEngine;

public class IceMelt : MonoBehaviour
{
    public float meltSpeed = 2f;

    private Transform iceTransform;
    private bool isMelting = false;

    void Start()
    {
        iceTransform = transform;
    }

    void Update()
    {
        if (isMelting)
        {
            Vector3 targetScale = new(1, 1, 0);
            iceTransform.localScale = Vector3.MoveTowards(
                iceTransform.localScale,
                targetScale,
                meltSpeed * Time.deltaTime
                );
        }
        else
        {
            Vector3 targetScale = new(1, 1, 1);
            iceTransform.localScale = Vector3.MoveTowards(
                iceTransform.localScale,
                targetScale,
                meltSpeed * Time.deltaTime
                );
        }
    }

    public void Melt() => isMelting = true;
    public void StopMelt() => isMelting = false;

}
