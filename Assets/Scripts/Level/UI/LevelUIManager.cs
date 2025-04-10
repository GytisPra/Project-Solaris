using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LevelUIManager : MonoBehaviour
{
    public Canvas levelUICanvas;
    public Canvas exitLevelPopupCanvas;
    public Canvas interactionPopupCanvas;
    public Canvas aboveCharCanvas;
    public Volume postProcessingVolume;
    public InteractTrigger interactTrigger;
    private DepthOfField depthOfField;

    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        if (!postProcessingVolume.profile.TryGet(out depthOfField))
        {
            Debug.LogError("Depth of field not found in Post Processing Volume!");
        }
    }


    public void SetLevelUICanvasActive(bool active)
    {
        levelUICanvas.gameObject.SetActive(active);
        SetDepthOfFieldEffectActive(active);
    }


    public void SetExitLevelPopupCanvasActive(bool active)
    {
        exitLevelPopupCanvas.gameObject.SetActive(active);

        SetDepthOfFieldEffectActive(active);
    }

    public void SetInteractionPopupActive(bool active)
    {
        interactionPopupCanvas.gameObject.SetActive(active);

        SetDepthOfFieldEffectActive(active);
    }
    
    public void SetAboveCharCanvasActive(bool active)
    {
        if (active && interactTrigger.IsCharInTrigger())
        {
            aboveCharCanvas.gameObject.SetActive(true);
        }
        else
        {
            aboveCharCanvas.gameObject.SetActive(false);
        }
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
