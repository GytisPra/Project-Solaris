using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    public int maxBounces = 5;
    public float rayLength = 10f;
    public LayerMask lensMask; // Set this in the Inspector
    private LineRenderer lineRenderer;
    public LayerMask mirrorMask;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
    }

    void Update()
    {
        CastLight(transform.position, transform.right);
    }

    void CastLight(Vector2 start, Vector2 direction)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(start);

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, direction, rayLength, lensMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                if (hit.collider.CompareTag("Mirror"))
                {
                    Debug.DrawRay(hit.point, hit.normal, Color.red, 2f); // Debug
                    direction = Vector2.Reflect(direction, hit.normal);
                    start = hit.point + direction * 0.01f; // Offset to prevent self-collision
                }
                else if (hit.collider.CompareTag("Lens"))
                {
                    Vector2 normal = hit.transform.up;
                    direction = Refract(direction, normal, 1.0f, 1.5f); // Air to Glass

                    start = hit.point + direction * 0.01f; // Offset slightly

                    // Simulate light exiting the lens
                    hit = Physics2D.Raycast(start, direction, rayLength, lensMask);
                    if (hit.collider != null)
                    {
                        normal = -hit.transform.up; // Reverse normal for exit
                        direction = Refract(direction, normal, 1.5f, 1.0f); // Glass to Air
                        start = hit.point + direction * 0.01f;
                    }
                }
            }
            else
            {
                points.Add(start + direction * rayLength);
                break;
            }
        }

        points = FilterVisiblePoints(points);

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
    List<Vector2> FilterVisiblePoints(List<Vector2> points)
    {
        List<Vector2> visiblePoints = new List<Vector2>();

        foreach (Vector2 point in points)
        {
            RaycastHit2D mirrorHit = Physics2D.Raycast(point, Vector2.zero, 0f, mirrorMask);

            if (mirrorHit.collider == null)  // If NOT behind a mirror, keep it
            {
                visiblePoints.Add(point);
            }
        }

        return visiblePoints;
    }
    Vector2 Refract(Vector2 incident, Vector2 normal, float n1, float n2)
    {
        float ratio = n1 / n2;
        float cosI = -Vector2.Dot(normal, incident);
        float sinT2 = ratio * ratio * (1 - cosI * cosI);

        if (sinT2 > 1.0f)
        {
            return Vector2.Reflect(incident, normal); // Total internal reflection
        }

        float cosT = Mathf.Sqrt(1 - sinT2);
        return ratio * incident + (ratio * cosI - cosT) * normal;
    }
}