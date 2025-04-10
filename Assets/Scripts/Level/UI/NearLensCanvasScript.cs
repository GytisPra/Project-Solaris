using UnityEngine;

public class NearLensCanvasScript : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    public void OpenInteractPopup()
    {
        levelUIManager.SetInteractionPopupActive(true);
        levelUIManager.SetLevelUICanvasActive(false);
        levelUIManager.SetNearLensCanvasActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);
    }
}
