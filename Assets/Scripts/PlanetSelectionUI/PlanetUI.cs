using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;

    private string currentPlanetName;
    private CameraRotation cameraRotation;

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();
        currentPlanetName = cameraRotation.GetCurrentTarget();

        if (cameraRotation == null)
        {
            Debug.LogError("cameraRotation component not found!");
        }
    }

    private void SaveLastPlanet()
    {
        PlayerPrefs.SetString("LastPlanet", cameraRotation.GetCurrentTarget());
        PlayerPrefs.Save();
    }

    public void Back()
    {
        SaveLastPlanet();

        SceneManager.LoadScene("MainMenuScene");
        SceneManager.UnloadSceneAsync(currentPlanetName);
    }

    public void ReturnToPlanetSelection()
    {
        SaveLastPlanet();

        if (planetSelectionUIManager != null) {
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(true);
            planetSelectionUIManager.SetPlanetUICanvasActive(false);
        } 
        else 
        {
            Debug.LogError("Planet selection UI manager not assigned in inspector!");
        }

        cameraRotation.ResetCamera();
    }

    public void ResetSolarSystemCamera()
    {
        cameraRotation.ResetCameraOnCurrentTarget();
    }
}
