using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        radius = Utils.GetSphereRadius(gameObject);
        prevRadius = radius;

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

        Vector3 localPosition = new(
            radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad),
            radius * Mathf.Sin(latRad),
            radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad)
        );

        Vector3 worldPosition = localPosition;

        GameObject point = Instantiate(pointPrefab, worldPosition, Quaternion.identity);
        point.name = $"{level.ID}";
        point.layer = 6;
        point.transform.parent = transform;
        point.SetActive(false);

        LevelData levelData = point.AddComponent<LevelData>();

        levelData.description = level.description;
        levelData.completed = level.completed;
        levelData.levelID = level.ID;
        levelData.planetName = gameObject.name;
        levelData.title = level.title;  
        levelData.completed = level.completed;

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

            Vector3 localPosition = new(
                radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad),
                radius * Mathf.Sin(latRad),
                radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad)
            );

            placedPoints[i].transform.position = transform.position + localPosition; // Adjust for planet's position
        }
    }
    public void ShowLevels(bool show)
    {
        if (show)
        {
            foreach (var point in placedPoints)
            {
                point.SetActive(show);
            }
        }
        else if (!show)
        {
            foreach (var point in placedPoints)
            {
                point.SetActive(show);
            }
        }
    }
}