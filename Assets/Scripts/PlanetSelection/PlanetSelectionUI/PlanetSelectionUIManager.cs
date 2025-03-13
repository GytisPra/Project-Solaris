using UnityEngine;

public class PlanetSelectionUIManager : MonoBehaviour
{
    public Canvas planetUICanvas;
    public Canvas planetSelectionCanvas;
    public Canvas levelUICanvas;
    public LevelUI levelUI;

    void Start()
    {
        levelUICanvas.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey("LastPlanet"))
        {
            planetSelectionCanvas.gameObject.SetActive(true);
            planetUICanvas.gameObject.SetActive(false);
        }
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

    public void OpenLevelUI(int LevelID) 
    {
        levelUI.DisplayLevelInfo(LevelID);
        levelUICanvas.gameObject.SetActive(true);
    }
}
