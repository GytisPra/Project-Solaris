using UnityEngine;

public class NeptuneInteractScript: MonoBehaviour
{
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private GameObject popup;
    public void ClosePopup()
    {
        popup.SetActive(false);
        levelUIManager.SetLevelUICanvasActive(true);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void OpenPopup()
    {
        popup.SetActive(true);
        levelUIManager.SetLevelUICanvasActive(false);

        GameStateManager.Instance.SetState(GameState.Menu);
    }
}