using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NewGameUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public PlanetsDatabase planetsDatabase;
    public PlanetSelectionUI planetSelectionUI;

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

        planetSelectionUI.ReloadButtons();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        StartCoroutine(PostClearUnlockedPlanets());

        planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }

    private IEnumerator PostClearUnlockedPlanets()
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm("https://project-solaris-shop-production.up.railway.app/clear-unlocks", "");

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
