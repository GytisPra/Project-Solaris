using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnInteractTriggerEnter : MonoBehaviour
{
    public Canvas interactionCanvas;
    private void OnTriggerEnter(Collider other)
    {
        interactionCanvas.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        interactionCanvas.gameObject.SetActive(false);
    }
}
