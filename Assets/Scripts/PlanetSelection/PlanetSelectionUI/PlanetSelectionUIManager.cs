using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PlanetSelectionUIManager : MonoBehaviour
{
    public Canvas planetUICanvas;
    public Canvas planetSelectionCanvas;
    public Canvas levelUICanvas;
    public Canvas mainMenuCanvas;
    public Canvas travelUICanvas;
    public Canvas newGamePopupCanvas;
    public Canvas optionsCanvas;
    public Canvas creditsCanvas;

    public LevelUI levelUI;

    public Volume postProcessingVolume;
    public CameraRotation cameraRotation;
    public PlanetInteraction planetInteraction;

    private DepthOfField depthOfField;

    void Start()
    {
        SetMainMenuCanvasActive(true);

        SetLevelUICanvasActive(false);
        SetPlanetSelectionCanvasActive(false);
        SetPlanetUICanvasActive(false);

        if (postProcessingVolume.profile.TryGet(out depthOfField))
        {
             Debug.LogError("No Depth of Field effect found in the Volume profile.");
        }
    }

    public void SetPlanetUICanvasActive(bool active)
    {
        planetUICanvas.gameObject.SetActive(active);

        if (active) {
            cameraRotation.EnableInput();
            planetInteraction.Enable();
        }
    }

    public void SetPlanetSelectionCanvasActive(bool active)
    {
        planetSelectionCanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
        }
    }

    public void SetLevelUICanvasActive(bool active)
    {
        levelUICanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
        }
    }

    public void SetMainMenuCanvasActive(bool active)
    {
        mainMenuCanvas.gameObject.SetActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
        }
    }

    public void SetTravelUICanvasActive(bool active)
    {
        travelUICanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
        }
    }

    public void OpenLevelUI(LevelData levelData)
    {
        levelUI.DisplayLevelInfo(levelData);
        levelUICanvas.gameObject.SetActive(true);
        SetDepthOfFieldEffectActive(true);

        cameraRotation.DisableInput();
        planetInteraction.Disable();
    }

    public void SetNewGamePopupCanvasActive(bool active)
    {
        newGamePopupCanvas.gameObject.SetActive(active);

        SetDepthOfFieldEffectActive(active);

        cameraRotation.DisableInput();
        planetInteraction.Disable();
    }

    public void SetOptionsCanvasActive(bool active)
    {
        optionsCanvas.gameObject.SetActive(active);

        SetDepthOfFieldEffectActive(active);

        cameraRotation.DisableInput();
        planetInteraction.Disable();
    }

    public void ClosePlanetSelectionUI()
    {
        planetSelectionCanvas.gameObject.SetActive(false);

        if (cameraRotation.GetCurrentTarget() == "")
        {
            SetMainMenuCanvasActive(true);
            SetDepthOfFieldEffectActive(false);
        }
        else
        {
            SetTravelUICanvasActive(true);
        }
    }

    public void SetCreditsCanvasActive(bool active)
    {
        creditsCanvas.gameObject.SetActive(active);

        SetDepthOfFieldEffectActive(active);

        cameraRotation.DisableInput();
        planetInteraction.Disable();
    }

    public void SetDepthOfFieldEffectActive(bool active)
    {
        if (depthOfField == null)
        {
            Debug.LogError("Depth of Field effect is not found in the Volume profile.");
            return;
        }

        depthOfField.active = active;
    }
}
