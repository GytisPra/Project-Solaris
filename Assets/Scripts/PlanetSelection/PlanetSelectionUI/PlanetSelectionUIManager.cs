using UnityEngine;

public class PlanetSelectionUIManager : MonoBehaviour
{
    public Canvas planetUICanvas;
    public Canvas planetSelectionCanvas;

    void Start()
    {
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
}
