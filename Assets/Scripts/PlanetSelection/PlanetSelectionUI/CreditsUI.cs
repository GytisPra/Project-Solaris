using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetCreditsCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }
}
