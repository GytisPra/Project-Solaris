using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class LoadLevelsOnPlanet : MonoBehaviour
{
    public TextAsset jsonFile;
    public float radius = 1f;
    public GameObject pointPrefab;
    public Color pointColor;

    private Levels levelsInJson;
    private readonly List<GameObject> placedPoints = new();
    private float prevRadius = 1f;

    void Start()
    {
        if (jsonFile != null)
        {
            levelsInJson = JsonUtility.FromJson<Levels>(jsonFile.text);
            foreach (Level level in levelsInJson.levels)
            {
                PlacePoint(level);
            }
        }
    }
    void Update()
    {
        AdjustPointsRadius();
    }

    void PlacePoint(Level level)
    {
        if (pointPrefab == null) return;

        float latRad = level.latitude * Mathf.Deg2Rad;
        float lonRad = level.longitude * Mathf.Deg2Rad;

        Vector3 position = new(
            radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad),
            radius * Mathf.Sin(latRad),
            radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad)
        );

        GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
        point.name = $"Level_{level.ID}";
        point.layer = 6;
        point.transform.parent = transform;
        point.GetComponent<Renderer>().material.color = pointColor;

        placedPoints.Add(point);
    }

    void AdjustPointsRadius()
    {
        if (placedPoints.Count == 0 || radius == prevRadius) return;

        prevRadius = radius;

        for (int i = 0; i < placedPoints.Count; i++)
        {
            Level level = levelsInJson.levels[i];

            float latRad = level.latitude * Mathf.Deg2Rad;
            float lonRad = level.longitude * Mathf.Deg2Rad;

            Vector3 newPosition = new(
                radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad),
                radius * Mathf.Sin(latRad),
                radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad)
            );

            placedPoints[i].transform.position = newPosition;
        }
    }
}