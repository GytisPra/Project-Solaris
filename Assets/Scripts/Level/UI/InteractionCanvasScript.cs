using UnityEngine;

public class InteractionCanvasScript : MonoBehaviour
{
    public LevelUIManager levelUIManager;

    public void OpenInteractPopup()
    {
        levelUIManager.SetInteractionPopupActive(true);
        levelUIManager.SetLevelUICanvasActive(false);
        levelUIManager.SetNearRheostatActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);

        GameStateManager.Instance.SetState(GameState.Menu);
    }
}
