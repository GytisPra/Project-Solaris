using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewGameUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public PlanetsDatabase planetsDatabase;
    public List<LevelsDatabase> levelsDatabases;
    public SubjectsDatabase subjectsDatabase;
    public PlanetHider planetHider;
    public CameraRotation cameraRotation;
    public GameObject earth;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }

    public void ResetData()
    {
        foreach(Planet planet in planetsDatabase.planets)
        {
            if (planet.planetName == "Earth") continue;

            planet.SetPlanetToLocked();
        }


        foreach (var levelsDatabase in levelsDatabases)
        {
            foreach (Level level in levelsDatabase.levels)
            {
                level.ResetLevel();
            }
        }

        foreach (var subject in subjectsDatabase.subjects)
        {
            subject.isUnlocked = false;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        planetHider.HideLockedPlanets();
        StartCoroutine(PostClearUnlockedPlanets());

        if (cameraRotation != null)
        {
            cameraRotation.SetTargetObject(earth);
        }
        else
        {
            Debug.LogError("Camera rotation component not set in inspector!");
        }

        if (planetSelectionUIManager != null)
        {
            planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
            planetSelectionUIManager.SetMainMenuCanvasActive(false);
            planetSelectionUIManager.SetPlanetUICanvasActive(true);
        }
        else
        {
            Debug.LogError("Planet selection UI manager not set in inspector!");

        }
        
    }

    private IEnumerator PostClearUnlockedPlanets()
    {
        string url;

#if UNITY_EDITOR
        url = "http://localhost:5173/clear-unlocks"; // Local dev server
#else
        url = "https://project-solaris-shop.vercel.app/clear-unlocks"; // Live server
#endif

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully cleared unlocked planets.");
        }
        else
        {
            Debug.LogError("Failed to clear unlocked planets: " + request.error);
        }
    }
}
