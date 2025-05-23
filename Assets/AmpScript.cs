using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
public class AmpScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI amps;
    [SerializeField] private Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((slider.value+5) != 0) 
        {
            amps.text = (Math.Round(4 / (slider.value + 5), 2)).ToString() + "A";
        }
        
    }
}
