using UnityEngine;

public class NewGameUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("LastPlanet");
        PlayerPrefs.DeleteKey("VolumeLevel");
        PlayerPrefs.Save();

        planetSelectionUIManager.SetNewGamePopupCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }
}
