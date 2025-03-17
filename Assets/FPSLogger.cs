using UnityEngine;

public class FPSLogger : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = -1;
    }
    void Update()
    {
        int fps = Mathf.RoundToInt(1f / Time.deltaTime); // Calculate FPS
        Debug.Log("FPS: " + fps);
    }
}
