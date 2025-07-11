using UnityEngine;

public class PlanetUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;

    private CameraRotation cameraRotation;

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();

        if (cameraRotation == null)
        {
            Debug.LogError("cameraRotation component not found!");
        }
    }

    private void SaveLastPlanet()
    {
        PlayerPrefs.SetString("LastPlanet", cameraRotation.GetCurrentTargetName());
        PlayerPrefs.Save();
    }

    public void Back()
    {
        SaveLastPlanet();

        if (planetSelectionUIManager != null)
        {
            planetSelectionUIManager.SetMainMenuCanvasActive(true);
            planetSelectionUIManager.SetPlanetUICanvasActive(false);
        }
        else
        {
            Debug.LogError("Planet selection UI manager not assigned in inspector!");
        }

        cameraRotation.ResetCamera();
    }

    public void GoToTravelMenu()
    {
        SaveLastPlanet();

        if (planetSelectionUIManager != null)
        {
            planetSelectionUIManager.SetTravelUICanvasActive(true);
            planetSelectionUIManager.SetPlanetUICanvasActive(false);
        }
        else
        {
            Debug.LogError("Planet selection UI manager not assigned in inspector!");
        }
    }

    public void ResetSolarSystemCamera()
    {
        cameraRotation.ResetCameraOnCurrentTarget();
    }
    public void OpenSolarPad()
    {
        SolarPad.Instance.Open();
    }
}
