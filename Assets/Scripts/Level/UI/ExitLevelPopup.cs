using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitLevelPopup : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    public void ClosePopUp()
    {
        levelUIManager.SetExitLevelPopupCanvasActive(false);
        levelUIManager.SetLevelUICanvasActive(true);

        levelUIManager.SetDepthOfFieldEffectActive(false);
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void ExitLevel()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        levelUIManager.SetExitLevelPopupCanvasActive(false);

        LoadingScreenManager.Instance.LoadScene("PlanetSelection");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlanetSelection")
        {
            MainMenuScript mainMenuScript = FindAnyObjectByType<MainMenuScript>();
            if (mainMenuScript != null)
            {
                mainMenuScript.StartGame();
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
