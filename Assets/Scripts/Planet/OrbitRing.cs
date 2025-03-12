using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitRing : MonoBehaviour
{
    public GameObject sun;
    public int segments = 100;
    public float lineWidth = 0.05f;
    public Color lineColor;

    private bool lineEnabled = true;
    private float prevLineWidth;
    private float orbitRadius;
    private LineRenderer lineRenderer;

    void Start()
    {
        if (!lineEnabled)
        {
            return;
        }

        prevLineWidth = lineWidth;

        orbitRadius = Mathf.Abs(transform.position.z - sun.transform.position.z);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1; // One extra point to close the circle
        lineRenderer.loop = true; // Make it a closed loop
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.endColor = lineColor;
        lineRenderer.startColor = lineColor;

        DrawOrbit();
    }

    public void SetLineVisibility(bool enabled)
    {
        lineEnabled = enabled;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = enabled;
        }
    }


    void Update()
    {
        if (lineRenderer == null) return;

        // Toggle visibility based on lineEnabled
        if (!lineEnabled)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
            return;
        }

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }

        lineRenderer.endColor = lineColor;
        lineRenderer.startColor = lineColor;

        if (lineWidth == prevLineWidth)
        {
            return;
        }

        prevLineWidth = lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    void DrawOrbit()
    {
        if (sun == null)
        {
            Debug.LogError("Sun object not assigned to OrbitRing!");
            return;
        }

        Vector3 center = sun.transform.position;
        float angleStep = 2 * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float x = center.x + Mathf.Cos(angle) * orbitRadius;
            float z = center.z + Mathf.Sin(angle) * orbitRadius;
            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }
}
