using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class RheostatSlider : MonoBehaviour
{

    public GameObject slidingObject;
    public Slider slider;
    public Vector3 vector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if ( slider.value != 0)
        {
            vector = new Vector3(0, -(slider.value / 5), 0);
        }

        slidingObject.transform.localPosition = vector;
    }
}
