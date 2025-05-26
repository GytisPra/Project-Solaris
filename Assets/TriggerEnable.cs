using UnityEngine;
using UnityEngine.UI;

public class TriggerEnable : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject EndPortal;

    private void Start()
    {
        EndPortal.GetComponent<SphereCollider>().isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value == 5) 
        { 
            EndPortal.GetComponent <SphereCollider>().isTrigger = true;
        }
    }
}
