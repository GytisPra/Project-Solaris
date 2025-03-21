using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject planets;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public CameraRotation cameraRotation;
    private string lastPlanet;

    public void StartGame()
    {
        if (!PlayerPrefs.HasKey("LastPlanet")) {
            cameraRotation.ResetCamera();
            planetSelectionUIManager.SetMainMenuCanvasActive(false);
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(true);
            return;
        }

        lastPlanet = PlayerPrefs.GetString("LastPlanet");
        GameObject planet = planets.transform.Find(lastPlanet).gameObject;

        if (lastPlanet == "Saturn") {
           planet = planet.transform.Find("SaturnST").gameObject;
        }

        if(planet == null) {
            Debug.LogError("Last Planet does not exist!");
            cameraRotation.ResetCamera();
            return;
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

    public void OpenOptions()
    {
        Debug.Log("Opening Options");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
