using UnityEngine;
using UnityEngine.Rendering;

public class FPSSetter : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        Application.targetFrameRate = 60;
    }
}
