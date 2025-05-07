using System;
using UnityEngine;

public class TravelUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;

    public void OpenPlanetSelection() {
        planetSelectionUIManager.SetTravelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetSelectionCanvasActive(true);
    }

    public void ReturnToPlanetView() {
        planetSelectionUIManager.SetTravelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetUICanvasActive(true);
    }
}
