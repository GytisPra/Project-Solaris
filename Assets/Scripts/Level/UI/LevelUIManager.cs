using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LevelUIManager : MonoBehaviour
{
    public Canvas levelUICanvas;
    public Canvas exitLevelPopupCanvas;
    public Canvas interactionCanvas;
    public Canvas interactionPopupCanvas;
    public Volume postProcessingVolume;
    private DepthOfField depthOfField;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;

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
