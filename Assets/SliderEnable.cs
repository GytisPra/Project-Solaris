using UnityEngine;

public class SliderEnable : MonoBehaviour
{
    public GameObject slider;
    public bool on;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        on = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SliderOnOff()
    {
        slider.SetActive(on);
        on = !on;
    }
}
