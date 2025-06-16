using Nobi.UiRoundedCorners;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject planets;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public CameraRotation cameraRotation;
    private string lastPlanet;
    public Button playButton;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("LastPlanet"))
        {
            playButton.interactable = false;
            playButton.GetComponentInChildren<TMP_Text>().color = new(0.5f, 0.5f, 0.5f, 1f);
            playButton.GetComponent<RoundedCornersWithOutline>().outlineColor = new(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            playButton.GetComponentInChildren<TMP_Text>().color = new(1f, 1f, 1f, 1f);
            playButton.GetComponent<RoundedCornersWithOutline>().outlineColor = new(1f, 1f, 1f, 1f);
            playButton.interactable = true;
        }
    }
    public void StartGame()
    {
        lastPlanet = PlayerPrefs.GetString("LastPlanet", "Earth");

        if (lastPlanet == "Sun" || lastPlanet == "") {
            cameraRotation.ResetCamera();
            planetSelectionUIManager.SetMainMenuCanvasActive(false);
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(true);
            return;
        }

        GameObject planet = planets.transform.Find(lastPlanet).gameObject;

        if(planet == null) {
            Debug.LogError("Last Planet does not exist!");
            cameraRotation.ResetCamera();
            return;
        }

        if (lastPlanet == "Saturn")
        {
            planet = planet.transform.GetChild(1).gameObject;
        }

        if (cameraRotation != null) {
            cameraRotation.SetTargetObject(planet);
        } else {
            Debug.LogError("Camera rotation component not set in inspector!");
        }

        if (planetSelectionUIManager != null) {
            planetSelectionUIManager.SetMainMenuCanvasActive(false);
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
            planetSelectionUIManager.SetPlanetUICanvasActive(true);
        }
        else {
            Debug.LogError("Planet selection UI manager not set in inspector!");

        }
    }

    public void OpenNewGamePopup()
    {
        planetSelectionUIManager.SetMainMenuCanvasActive(false);
        planetSelectionUIManager.SetNewGamePopupCanvasActive(true);
    }

    public void OpenOptions()
    {
        planetSelectionUIManager.SetMainMenuCanvasActive(false);
        planetSelectionUIManager.SetOptionsCanvasActive(true);
    }

    public void OpenCredits()
    {
        planetSelectionUIManager.SetMainMenuCanvasActive(false);
        planetSelectionUIManager.SetCreditsCanvasActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
