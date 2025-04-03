using UnityEngine;

public class InteractionPopup : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    public void ClosePopUp()
    {
        levelUIManager.SetExitLevelPopupCanvasActive(false);
        levelUIManager.SetLevelUICanvasActive(true);

        levelUIManager.SetDepthOfFieldEffectActive(false);
    }
}
