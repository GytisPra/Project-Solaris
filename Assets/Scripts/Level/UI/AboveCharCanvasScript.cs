using UnityEngine;

public class AboveCharCanvasScript : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    public void OpenInteractPopup()
    {
        levelUIManager.SetInteractionPopupActive(true);
        levelUIManager.SetLevelUICanvasActive(false);
        levelUIManager.SetAboveCharCanvasActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);
    }
}
