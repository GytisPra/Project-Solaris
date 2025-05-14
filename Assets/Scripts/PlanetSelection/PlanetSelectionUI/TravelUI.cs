using System;
using UnityEngine;

public class TravelUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    private CameraRotation cameraRotation;

    private void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();
    }

    public void OpenPlanetSelection() {
        planetSelectionUIManager.SetTravelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetSelectionCanvasActive(true);
    }

    public void ReturnToPlanetView() {
        planetSelectionUIManager.SetTravelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetUICanvasActive(true);
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
            }
            else
            {
                Debug.LogError("planetSelectionUIManager not assigned in the inspector!");
            }
        }
        else
        {
            Debug.LogError($"Planet not provided!");
        }
    }
}
