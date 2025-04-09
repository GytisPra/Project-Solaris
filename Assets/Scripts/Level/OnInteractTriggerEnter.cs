using UnityEngine;

public class OnInteractTriggerEnter : MonoBehaviour
{
    private bool disabled = false;

    public LevelUIManager levelUIManager;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (disabled) return;

        levelUIManager.SetLevelUICanvasActive(false);

        levelUIManager.SetInteractionPopupActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(true);
    }
    public void DisableTrigger() => disabled = true;
    public void EnableTrigger() => disabled = false;
}
