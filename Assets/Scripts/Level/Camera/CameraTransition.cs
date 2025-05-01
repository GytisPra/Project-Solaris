using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraTransition : MonoBehaviour
{
    public static CameraTransition Instance { get; private set; }

    private Camera previousCamera;
    private Camera currentCamera;

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

    public IEnumerator SmoothCameraTransition(Camera fromCamera, Camera toCamera, float duration = 2f, GameState stateAfterTransition = GameState.Gameplay)
    {
        // At the start of the transition setState to cutscene
        // At the end we set it to gameplay to enable controls
        GameStateManager.Instance.SetState(GameState.Cutscene);

        previousCamera = fromCamera;
        currentCamera = toCamera;

        float elapsedTime = 0f;

        fromCamera.transform.GetPositionAndRotation(out Vector3 startingPosition, out Quaternion startingRotation);
        toCamera.transform.GetPositionAndRotation(out Vector3 targetPosition, out Quaternion targetRotation);

        toCamera.transform.SetPositionAndRotation(startingPosition, startingRotation);
        fromCamera.gameObject.SetActive(false);
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

        GameStateManager.Instance.SetState(stateAfterTransition);
    }

    public IEnumerator TransitionBack(float duration = 2f, GameState stateAfterTransition = GameState.Gameplay)
    {
        if (previousCamera == null)
        {
            Debug.LogError("There is no previous camera to transition back to.");
        }

        if (currentCamera == null)
        {
            Debug.LogError("Current camera is not set.");
        }

        yield return StartCoroutine(SmoothCameraTransition(currentCamera, previousCamera, duration, stateAfterTransition));
    }
}
