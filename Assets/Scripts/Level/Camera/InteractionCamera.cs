using UnityEngine;

public class InteractionCamera : MonoBehaviour
{
    [SerializeField] private Camera parentCamera;
    [SerializeField] private Transform interactionCanvas;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.5f;

    private float minFOV = 15f;
    private float maxFOV = 50f;

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
            minFOV = thirdPersonCamera.minFOV + 10f;
            maxFOV = thirdPersonCamera.maxFOV - 10f;
        }
    }

    void Update()
    {
        if (!thisCamera || !interactionCanvas || !parentCamera)
            return;

        thisCamera.fieldOfView = parentCamera.fieldOfView; 

        float normalizedFov = Mathf.InverseLerp(minFOV, maxFOV, thisCamera.fieldOfView);
        float targetScale = Mathf.Lerp(minScale, maxScale, normalizedFov);

        interactionCanvas.localScale = Vector3.one * targetScale;
    }
}
