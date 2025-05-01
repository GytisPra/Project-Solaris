using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    private bool disabled = false;
    private bool isCharInTrigger = false;

    public Canvas interactCanvas;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (disabled) return;

        interactCanvas.gameObject.SetActive(true);
        isCharInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        interactCanvas.gameObject.SetActive(false);
        isCharInTrigger = false;
    }

    /// <summary>
    /// Will display the interact canvas only if the char is still in the trigger
    /// </summary>
    public void ShowInteract()
    {
        if (isCharInTrigger)
        {
            interactCanvas.gameObject.SetActive(true);
        }
        else
        {
            interactCanvas.gameObject.SetActive(false);
        }
    }
    public void DisableTrigger() => disabled = true;
    public void EnableTrigger() => disabled = false;
    public bool IsCharInTrigger() => isCharInTrigger;
}
