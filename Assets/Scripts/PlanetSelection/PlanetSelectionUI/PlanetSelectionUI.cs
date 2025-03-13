using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSelectionUI : MonoBehaviour
{
    public GameObject planets;
    public PlanetSelectionUIManager planetSelectionUIManager;

    private CameraRotation cameraRotation;

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();
    }

    public void MoveToPlanet(string planetName)
    {
        GameObject planet = planets.transform.Find(planetName).gameObject;

        if (planetName == "Saturn") {
           planet = planet.transform.Find("SaturnST").gameObject;
        }

        if (planet != null)
        {
            PlayerPrefs.SetString("LastPlanet", planetName);
            PlayerPrefs.Save();

            cameraRotation.SetTargetObject(planet);

            if (planetSelectionUIManager != null)
            {
                planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                planetSelectionUIManager.SetPlanetUICanvasActive(true);
            } else {
                Debug.LogError("planetSelectionUIManager not assigned in the inspector!");
            }
        }
        else
        {
            Debug.LogError($"Planet '{planetName}' not found!");
        }
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ResetSolarSystemCamera() {
        cameraRotation.ResetCamera();
    }
}
