using UnityEngine;

public class InteractionCamera : MonoBehaviour
{
    [SerializeField] private Camera parentCamera;
    [SerializeField] private Transform interactionCanvas;
    private Camera thisCamera;
    private float prevFov = 30f;
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
    }

    void Update()
    {
        if (!thisCamera || !interactionCanvas)
        {
            return;
        }

        if (thisCamera.fieldOfView == parentCamera.fieldOfView)
        {
            return;
        }

        prevFov = thisCamera.fieldOfView;
        thisCamera.fieldOfView = parentCamera.fieldOfView;


        if (interactionCanvas)
        {
            float fovDelta = (thisCamera.fieldOfView - prevFov) / 45;

            interactionCanvas.localScale = new(
                interactionCanvas.localScale.x + fovDelta,
                interactionCanvas.localScale.y + fovDelta,
                interactionCanvas.localScale.z + fovDelta
                );
        }
    }
}
