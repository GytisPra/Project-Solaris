using System;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class OrbitRing : MonoBehaviour
{
    
    public float trailTime = 5f;
    public float trailWidth = 0.05f;
    public float minWidth = 0.05f;
    public float maxWidth = 0.1f;
    public Gradient trailColorGradient;
    public LevelsDatabase levelsDatabase;
    public PlanetsDatabase planetsDatabase;

    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        if (levelsDatabase == null)
        {
            trail.time = trailTime;
            trail.startWidth = trailWidth;
            trail.endWidth = 0;
            trail.colorGradient = trailColorGradient;
            return;
        }

        var planet = Array.Find(planetsDatabase.planets, p => p.planetName == gameObject.name);


        if (planet == null)
        {
            Debug.LogError($"Orbit trail could not be shown because planet was not found in database with the name: {gameObject.name}!");
            return;
        }

        planet.LoadUnlockData();

        // Display trail only if all the levels have been completed
        if (planet.numOfCompletedLevels >= levelsDatabase.levels.Length)
        {
            trail.time = trailTime;
            trail.startWidth = trailWidth;
            trail.endWidth = 0;
            trail.colorGradient = trailColorGradient;
        }
        else
        {
            // Hide the trail
            trail.Clear();
            trail.time = 0f;
        }
    }
}
