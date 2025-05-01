using System.Collections.Generic;
using UnityEngine;

public class LoadLevelsOnPlanet : MonoBehaviour
{
    public LevelsDatabase levelsDatabase;
    public float radius = 1f;
    public GameObject pointPrefab;

    private readonly List<GameObject> placedPoints = new();
    private string planetName;
    private float prevRadius = 1f;

    void Start()
    {
        planetName = gameObject.name;
    }

    void Update()
    {
        AdjustPointsRadius();
    }

    private void PlacePoints()
    {
        if (levelsDatabase != null)
        {
            foreach (Level level in levelsDatabase.levels)
            {
                if (PlayerPrefs.HasKey($"{planetName}_{level.title}_{level.ID}"))
                {
                    level.completed = true;
                }

                PlacePoint(level);
            }
        }
    }

    void PlacePoint(Level level)
    {
        radius = Utils.GetRadius(gameObject) * 1f;
        prevRadius = radius;

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
        point.transform.position = transform.position + localPosition;

        point.name = $"Level_{level.ID}";
        point.layer = 6;

        point.transform.SetParent(transform);
        point.transform.localScale = new(transform.localScale.x * radius, transform.localScale.y * radius, transform.localScale.z * radius);

        point.AddComponent<FaceCamera>();
        point.SetActive(false);

        SpriteRenderer fillRenderer = point.transform.GetChild(0).GetComponent<SpriteRenderer>();

        fillRenderer.material.color = new(1f, 1f, 1f, 0.2f);

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
            Level level = levelsDatabase.levels[i];

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
            if (placedPoints.Count <= 0)
            {
                PlacePoints();
            }

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