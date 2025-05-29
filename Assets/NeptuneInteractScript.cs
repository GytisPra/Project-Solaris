using System.Collections;
using UnityEngine;

public class NeptuneInteractScript: MonoBehaviour
{
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private GameObject popup;

    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera experimentViewCamera;
    public void ClosePopup()
    {
        popup.SetActive(false);
        levelUIManager.SetLevelUICanvasActive(true);

        StartCoroutine(
            CameraTransition.Instance.TransitionBack(0.5f)
        );
    }

    public void OpenPopup()
    {
        StartCoroutine(TransitionCameras());
    }

    private IEnumerator TransitionCameras()
    {
        yield return StartCoroutine(
            CameraTransition.Instance.
                SmoothCameraTransition(thirdPersonCamera, experimentViewCamera, 0.5f, GameState.Menu)
            );

        popup.SetActive(true);
        levelUIManager.SetLevelUICanvasActive(false);
    }
}