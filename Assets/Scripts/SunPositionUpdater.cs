using UnityEngine;

[ExecuteAlways]
public class SunPositionUpdater : MonoBehaviour
{
    public Transform sunTransform;

    void Update()
    {
        Shader.SetGlobalVector("_SunPosition", sunTransform.position);
    }
}
