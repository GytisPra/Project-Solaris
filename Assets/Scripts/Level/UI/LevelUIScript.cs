using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIScript : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    public void Back()
    {
        levelUIManager.SetLevelUICanvasActive(false);

        levelUIManager.SetExitLevelPopupCanvasActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(true);

        GameStateManager.Instance.SetState(GameState.Menu);
    }

    public void OpenSolarPad()
    {
        SolarPad.Instance.Open();
    }
}
