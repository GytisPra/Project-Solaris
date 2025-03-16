using UnityEngine;

public class PlanetSelectionUIManager : MonoBehaviour
{
    public Canvas planetUICanvas;
    public Canvas planetSelectionCanvas;
    public Canvas levelUICanvas;
    public Canvas mainMenuCanvas;
    public LevelUI levelUI;
    public Volume postProcessingVolume;
    private DepthOfField depthOfField;

    void Start()
    {
        mainMenuCanvas.gameObject.SetActive(true);

        levelUICanvas.gameObject.SetActive(false);
        planetSelectionCanvas.gameObject.SetActive(false);
        planetUICanvas.gameObject.SetActive(false);

    }

    public void SetPlanetUICanvasActive(bool active)
    {
        planetUICanvas.gameObject.SetActive(active);
    }

    public void SetPlanetSelectionCanvasActive(bool active)
    {
        planetSelectionCanvas.gameObject.SetActive(active);
    }

    public void SetLevelUICanvasActive(bool active)
    {
        levelUICanvas.gameObject.SetActive(active);
    }

    public void SetMainMenuCanvasActive(bool active)
    {
        mainMenuCanvas.gameObject.SetActive(active);
    }

    public void OpenLevelUI(int LevelID)
    {
        levelUI.DisplayLevelInfo(LevelID);
        levelUICanvas.gameObject.SetActive(true);
    }

    public void ToggleDepthOfFieldEffect()
    {
        if (bloom != null)
        {
            // Toggle the active state of Bloom effect
            bloom.active = !bloom.active;
            Debug.Log("Bloom effect toggled: " + bloom.active);
        }
    }
}
