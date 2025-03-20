using UnityEngine;

public class PlanetSelectionUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public GameObject popup;
    public GameObject planetSelection;

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
        planetSelectionUIManager.ClosePlanetSelectionUI();
    }

    public void ResetSolarSystemCamera() {
        cameraRotation.ResetCamera();
    }

    public void ShowPopup(bool show)
    {
        popup.SetActive(show);
        planetSelection.SetActive(!show);
    }
}
