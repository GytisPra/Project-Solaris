using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class UnlockEntry
{
    public string planetName;
    public string date; // keep as string for JSON parsing

    public DateTime GetParsedDate()
    {
        DateTime parsedDate;
        if (DateTime.TryParse(date, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedDate))
        {
            return parsedDate;
        }

        Debug.LogWarning($"Could not parse date: {date}");
        return DateTime.MinValue;
    }
}

[System.Serializable]
public class UnlockResponse
{
    public UnlockEntry[] unlocked;
}

public class UnlockManager : MonoBehaviour
{
    public PlanetsDatabase planetsDatabase;

    private Dictionary<string, Planet> planetLookup;

    void Awake()
    {
        // Build lookup once when this component is initialized
        planetLookup = new Dictionary<string, Planet>();
        foreach (var planet in planetsDatabase.planets)
        {
            planetLookup[planet.planetName] = planet;
        }
    }

    public IEnumerator FetchAndUnlockPlanets()
    {
        string url;

#if UNITY_EDITOR
        url = "http://localhost:5173/unlocks"; // Local dev server
#else
        url = "https://project-solaris-shop-production.up.railway.app/unlocks"; // Live server
#endif

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Unlock data: " + request.downloadHandler.text);

            var json = request.downloadHandler.text;
            UnlockResponse data = JsonUtility.FromJson<UnlockResponse>(json);

            foreach (UnlockEntry entry in data.unlocked)
            {
                if (entry.planetName == "Earth")
                {
                    continue;
                }

                var parsedDate = entry.GetParsedDate();
                if (parsedDate == DateTime.MinValue)
                    continue;

                if (!planetLookup.TryGetValue(entry.planetName, out var planet))
                {
                    Debug.LogWarning($"Planet not found: {entry.planetName}");
                    continue;
                }

                planet.LoadUnlockData();

                if (planet.IsAlreadyUnlockedAt(parsedDate))
                    continue;

                Debug.Log($"Unlocking planet: {entry.planetName} until {parsedDate}");
                planet.SetPlanetToUnlocked(parsedDate);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"Failed to fetch unlocks: {request.error}. The local dev server is probably not running!");
            foreach (var planet in planetsDatabase.planets)
            {
                planet.SetPlanetToUnlockedNoPrefs(DateTime.MaxValue);
            }

            yield return null;
#else
            Debug.LogError($"Failed to fetch unlocks: {request.error}");
#endif
        }
    }
}

