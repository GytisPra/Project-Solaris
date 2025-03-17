using System;
using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public TextMeshProUGUI levelDescription;
    public TextMeshProUGUI levelID;
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI levelState;
    public TextAsset[] jsonFiles;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetLevelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetUICanvasActive(true);
        planetSelectionUIManager.SetDepthOfFieldEffectActive(false);
    }

    public void DisplayLevelInfo(LevelData levelData)
    {
        levelDescription.text = levelData.description;
        levelID.text = levelData.levelID.ToString();
        levelTitle.text = levelData.title;

        if (!levelData.completed)
        {
            levelState.text = "Incomplete";
            levelState.color = new(1f, 0.8901961f, 0.2588235f, 1f);
        } 
        else
        {
            levelState.text = "Completed";
            levelState.color = new(0.5f, 1f, 0.3f, 1f);
        }
    }
}
