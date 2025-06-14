using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlanetHider : MonoBehaviour
{
    public PlanetsDatabase planetsDatabase;
    public List<GameObject> planets;

    void Start()
    {
        HideLockedPlanets();
    }

    public void HideLockedPlanets()
    {
        foreach (var planet in planets)
        {
            // Skip earth as it is not in the database
            if (planet.name == "Earth")
            {
                continue;
            }

            Planet databaseEntry = planetsDatabase.planets.FirstOrDefault(p => string.Equals(p.planetName, planet.name));

            if (databaseEntry == null)
            {
                Debug.LogError($"Planet whith the name {planet.name} database entry not found!");
                continue;
            }

            databaseEntry.LoadUnlockData();
            if (!databaseEntry.isFree && databaseEntry.IsPlanetPassOver())
            {
                int LayerHidden = LayerMask.NameToLayer("Hidden");
                planet.layer = LayerHidden;

                if (planet.transform.childCount > 0)
                {
                    for (int i = 0; i < planet.transform.childCount; i++)
                    {
                        var child = planet.transform.GetChild(i);
                        child.gameObject.layer = LayerHidden;
                    }
                }
            }
        }
    }

    public void UnhidePlanet(Planet planet)
    {
        var planetGameObject = planets.Find(p => string.Equals(p.name, planet.planetName));
        if (planetGameObject != null)
        {
            int LayerDefault = LayerMask.NameToLayer("Planet");
            planetGameObject.layer = LayerDefault;

            if (planetGameObject.transform.childCount > 0)
            {
                for (int i = 0; i < planetGameObject.transform.childCount; i++)
                {
                    var child = planetGameObject.transform.GetChild(i);
                    child.gameObject.layer = LayerDefault;
                }
            }
        }
        else
        {
            Debug.LogError($"Planet with the name {planet.planetName} game object could not be found!");
        }
    }

    public void UnhidePlanet(string planetName)
    {
        var planetGameObject = planets.Find(p => string.Equals(p.name, planetName));
        if (planetGameObject != null)
        {
            int LayerDefault = LayerMask.NameToLayer("Planet");
            planetGameObject.layer = LayerDefault;

            if (planetGameObject.transform.childCount > 0)
            {
                for (int i = 0; i < planetGameObject.transform.childCount; i++)
                {
                    var child = planetGameObject.transform.GetChild(i);
                    child.gameObject.layer = LayerDefault;
                }
            }
        }
        else
        {
            Debug.LogError($"Planet with the name {planetName} game object could not be found!");
        }
    }
}
