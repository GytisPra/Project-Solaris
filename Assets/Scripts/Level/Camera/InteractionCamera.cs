using UnityEngine;

public class InteractionCamera : MonoBehaviour
{
    [SerializeField] private Camera parentCamera;
    [SerializeField] private Transform interactionCanvas;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.5f;

    private float minDistance = 15f;
    private float maxDistance = 20f;

    private Camera thisCamera;

    void Start()
    {
        if (parentCamera == null)
        {
            Debug.LogError("Parent camera not assigned in the inspector!");
        }


        if (!transform.TryGetComponent<Camera>(out thisCamera))
        {
            Debug.LogError("Camera component not found!");
        }

        if(parentCamera.TryGetComponent<ThirdPersonCamera>(out var thirdPersonCamera))
        {
            minDistance = thirdPersonCamera.minZoomDistance + 10f;
            maxDistance = thirdPersonCamera.maxZoomDistance - 10f;
        }
    }

    void Update()
    {
        if (!thisCamera || !interactionCanvas || !parentCamera)
            return;

        thisCamera.fieldOfView = parentCamera.fieldOfView; 

        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, thisCamera.transform.position.z);
        float targetScale = Mathf.Lerp(minScale, maxScale, normalizedDistance);

        interactionCanvas.localScale = Vector3.one * targetScale;
    }
}
