using System.Collections;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public static CameraTransition Instance { get; private set; }

    [SerializeField] private PlayerController playerController;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public IEnumerator SmoothCameraTransition(Camera fromCamera, Camera toCamera, bool turnControlsOff, float duration = 2f)
    {
        float elapsedTime = 0f;

        if (turnControlsOff)
        {
            playerController.Disable();
            thirdPersonCamera.gameObject.SetActive(false);
        }


        fromCamera.transform.GetPositionAndRotation(out Vector3 startingPosition, out Quaternion startingRotation);
        toCamera.transform.GetPositionAndRotation(out Vector3 targetPosition, out Quaternion targetRotation);

        toCamera.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            toCamera.transform.SetPositionAndRotation(
                Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration),
                Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / duration)
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        toCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);
        fromCamera.gameObject.SetActive(false);

        if (!turnControlsOff)
        {
            playerController.Enable();
            thirdPersonCamera.gameObject.SetActive(true);
        }
    }
}
