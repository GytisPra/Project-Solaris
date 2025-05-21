using UnityEngine;

public class CanvasDistanceScaler : MonoBehaviour
{
    [SerializeField] private Transform interactionCanvas;
    [SerializeField] private Camera parentCamera;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.5f;

    [SerializeField] private float scaleMinDistance = 10f; // start scaling at this distance
    [SerializeField] private float scaleMaxDistance = 15f; // reach maxScale at this distance

    private ThirdPersonCamera thirdPersonCamera;
    private Camera thisCamera;

    void Start()
    {
        thisCamera = GetComponent<Camera>();

        if (!parentCamera)
        {
            Debug.LogError("Parent camera not assigned!");
            return;
        }

        thirdPersonCamera = parentCamera.GetComponent<ThirdPersonCamera>();

        if (!thirdPersonCamera)
        {
            Debug.LogError("ThirdPersonCamera component not found on parent camera!");
        }
    }

    void Update()
    {
        if (!interactionCanvas || !parentCamera || !thirdPersonCamera)
            return;

        if (thisCamera.fieldOfView != parentCamera.fieldOfView)
        {
            thisCamera.fieldOfView = parentCamera.fieldOfView;
        }

        float distance = Vector3.Distance(parentCamera.transform.position, interactionCanvas.position);

        float t = Mathf.InverseLerp(scaleMinDistance, scaleMaxDistance, distance);

        float scaleValue = Mathf.Lerp(minScale, maxScale, t);

        interactionCanvas.localScale = Vector3.one * scaleValue;
    }
}
