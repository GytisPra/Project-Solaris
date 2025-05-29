using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueOhm : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI amount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        amount.text = (slider.value + 5).ToString();
    }
}
