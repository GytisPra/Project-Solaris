using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer fillRenderer;
    private SpriteRenderer outlineRenderer;
    private Color originalFillColor;
    private Color originalOutlineColor;

    [Range(0f, 1f)] public float darkenFactor = 0.7f;

    private void Start()
    {
        mainCamera = Camera.main;
        fillRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        outlineRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        if (fillRenderer != null)
        {
            originalFillColor = fillRenderer.material.color;
            originalOutlineColor = outlineRenderer.material.color;
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);

            CheckVisibility();
        }
    }

    void CheckVisibility()
    {
        if (fillRenderer == null || outlineRenderer == null) return;

        Vector3 directionToObject = transform.position - mainCamera.transform.position;

        if (Physics.Raycast(mainCamera.transform.position, directionToObject.normalized, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == gameObject)
            {
                fillRenderer.material.color = originalFillColor;
                outlineRenderer.material.color = originalOutlineColor;
            }
            else
            {
                fillRenderer.material.color = DarkenColor(originalFillColor, darkenFactor);
                outlineRenderer.material.color = DarkenColor(originalOutlineColor, darkenFactor);
            }
        }
    }

    Color DarkenColor(Color color, float factor)
    {
        return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
    }
}
