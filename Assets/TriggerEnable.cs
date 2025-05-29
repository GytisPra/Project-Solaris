using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriggerEnable : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject EndPortal;
    [SerializeField] private Button interactButton;
    [SerializeField] private GameObject completed;
    [SerializeField] private Renderer portalTopRenderer;

    private Material portalTopMaterial;

    private void Start()
    {
        EndPortal.GetComponent<SphereCollider>().isTrigger = false;
        portalTopMaterial = portalTopRenderer.materials[1];

        StartCoroutine(FlickerEmissionRandomly());
    }

    private IEnumerator FlickerEmissionRandomly()
    {
        int flickerCount = Random.Range(20, 30); // Flicker 20 to 30 times

        for (int i = 0; i < flickerCount; i++)
        {
            // Turn on emission
            portalTopMaterial.EnableKeyword("_EMISSION");
            portalTopMaterial.SetColor("_EmissionColor", Color.white);

            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

            // Turn off emission
            portalTopMaterial.SetColor("_EmissionColor", Color.black);
            portalTopMaterial.DisableKeyword("_EMISSION");

            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        }
    }

    public void OnValueChange()
    {
        if (slider.value == 5)
        {
            EndPortal.GetComponent<SphereCollider>().isTrigger = true;
            interactButton.gameObject.SetActive(false);
            completed.SetActive(true);

            portalTopMaterial.EnableKeyword("_EMISSION");
            portalTopMaterial.SetColor("_EmissionColor", Color.white);
        }
        else
        {
            EndPortal.GetComponent<SphereCollider>().isTrigger = false;
            interactButton.gameObject.SetActive(true);
            completed.SetActive(false);

            portalTopMaterial.SetColor("_EmissionColor", Color.black);
            portalTopMaterial.DisableKeyword("_EMISSION");
        }
    }
}
