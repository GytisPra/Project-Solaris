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

    public void MoveToPlanet(GameObject planet)
    {
        if (planet != null)
        {
            PlayerPrefs.SetString("LastPlanet", planet.name);
            PlayerPrefs.Save();

            cameraRotation.SetTargetObject(planet);

            if (planetSelectionUIManager != null)
            {
                planetSelectionUIManager.SetTravelUICanvasActive(false);
                planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                planetSelectionUIManager.SetPlanetUICanvasActive(true);
            } else {
                Debug.LogError("planetSelectionUIManager not assigned in the inspector!");
            }
        }
        else
        {
            Debug.LogError($"Planet not provided!");
        }
    }

    public void ReturnToTravelMenu() {
        if (planetSelectionUIManager != null)
        {
            planetSelectionUIManager.SetTravelUICanvasActive(true);
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
        }
        else
        {
            Debug.LogError("Planet selection UI manager not assigned in inspector!");
        }
    }

    public void ResetSolarSystemCamera() {
        cameraRotation.ResetCamera();
    }
}
