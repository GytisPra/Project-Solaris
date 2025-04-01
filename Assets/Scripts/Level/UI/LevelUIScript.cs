using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIScript : MonoBehaviour
{
    public LevelUIManager levelUIManager;
    public void Back()
    {
        levelUIManager.SetLevelUICanvasActive(false);

        levelUIManager.SetExitLevelPopupCanvasActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(true);
    }
}
