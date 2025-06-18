using Nobi.UiRoundedCorners;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class PlanetSelectionUIManager : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas planetUICanvas;
    public Canvas planetSelectionCanvas;
    public Canvas levelUICanvas;
    public Canvas mainMenuCanvas;
    public Canvas travelUICanvas;
    public Canvas newGamePopupCanvas;
    public Canvas optionsCanvas;
    public Canvas creditsCanvas;

    [Header("Scripts")]
    public LevelUI levelUI;
    public CameraRotation cameraRotation;
    public PlanetInteraction planetInteraction;

    [Header("Graphics")]
    public Volume postProcessingVolume;
    private DepthOfField depthOfField;

    public Button playButton;

    void Start()
    {
        if (!postProcessingVolume.profile.TryGet(out depthOfField))
        {
            Debug.LogError("Depth of field not found in Post Processing Volume!");
        }
    }

    public void SetPlanetUICanvasActive(bool active)
    {
        planetUICanvas.gameObject.SetActive(active);

        if (active) {
            cameraRotation.EnableInput();
            planetInteraction.Enable();
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }

    public void SetPlanetSelectionCanvasActive(bool active)
    {
        if (active) 
        {
            planetSelectionCanvas.gameObject.SetActive(true);
            SetDepthOfFieldEffectActive(true);
            cameraRotation.DisableInput();
            planetInteraction.Disable();
            Screen.orientation = ScreenOrientation.Portrait;
        } 
        else
        {
            planetSelectionCanvas.gameObject.SetActive(false);
            SetDepthOfFieldEffectActive(false);
        }
    }

    public void SetLevelUICanvasActive(bool active)
    {
        levelUICanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }

    public void SetMainMenuCanvasActive(bool active)
    {
        mainMenuCanvas.gameObject.SetActive(active);

        if (active) {
            if (!PlayerPrefs.HasKey("LastPlanet"))
            {
                playButton.interactable = false;
                playButton.GetComponentInChildren<TMP_Text>().color = new(0.5f, 0.5f, 0.5f, 1f);
                playButton.GetComponent<RoundedCornersWithOutline>().outlineColor = new(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                playButton.GetComponentInChildren<TMP_Text>().color = new(1f, 1f, 1f, 1f);
                playButton.GetComponent<RoundedCornersWithOutline>().outlineColor = new(1f, 1f, 1f, 1f);
                playButton.interactable = true;
            }
            cameraRotation.DisableInput();
            planetInteraction.Disable();
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    public void SetTravelUICanvasActive(bool active)
    {
        travelUICanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);

        if (active) {
            cameraRotation.DisableInput();
            planetInteraction.Disable();
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    public void OpenLevelUI(LevelData levelData, SpriteRenderer fillRenderer)
    {
        levelUI.DisplayLevelInfo(levelData, fillRenderer);
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

        if (cameraRotation.GetCurrentTargetName() == "")
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
            return;
        }

        depthOfField.active = active;
    }
}
