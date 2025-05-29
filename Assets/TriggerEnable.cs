using UnityEngine;
using UnityEngine.UI;

public class TriggerEnable : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject EndPortal;
    [SerializeField] private Button interactButton;
    [SerializeField] private GameObject completed;

    private void Start()
    {
        EndPortal.GetComponent<SphereCollider>().isTrigger = false;
    }

    public void OnValueChange()
    {
        if (slider.value == 5) 
        { 
            EndPortal.GetComponent <SphereCollider>().isTrigger = true;
            interactButton.gameObject.SetActive(false);
            completed.SetActive(true);
        }
        else
        {
            EndPortal.GetComponent<SphereCollider>().isTrigger = false;
            interactButton.gameObject.SetActive(true);
            completed.SetActive(false);
        }
    }
}
