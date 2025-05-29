using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NewGameUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public PlanetsDatabase planetsDatabase;
    public LevelsDatabase earthLevelsDatabase;

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

        foreach (Level level in earthLevelsDatabase.levels)
        {
            level.ResetLevel();
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        StartCoroutine(PostClearUnlockedPlanets());

        planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
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
