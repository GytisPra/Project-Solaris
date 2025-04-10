using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    private bool disabled = false;
    private bool isCharInTrigger = false;

    public Canvas aboveCharCanvas;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (disabled) return;

        aboveCharCanvas.gameObject.SetActive(true);
        isCharInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        aboveCharCanvas.gameObject.SetActive(false);
        isCharInTrigger = false;
    }
    public void DisableTrigger() => disabled = true;
    public void EnableTrigger() => disabled = false;
    public bool IsCharInTrigger() => isCharInTrigger;
}
