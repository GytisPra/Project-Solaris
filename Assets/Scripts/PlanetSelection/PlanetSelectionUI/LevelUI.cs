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

    private Color intialColor;
    private SpriteRenderer fillRenderer;
    private LevelData currLevelData;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetLevelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetUICanvasActive(true);
        planetSelectionUIManager.SetDepthOfFieldEffectActive(false);
        fillRenderer.material.color = intialColor;
    }

    public void OpenLevel()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        LevelDataHolder.currLevelData = currLevelData;

        PlayerPrefs.SetString("levelPlanet", currLevelData.planetName);
        PlayerPrefs.SetString("lastPlanet", currLevelData.planetName);
        PlayerPrefs.SetInt("levelID", currLevelData.levelID);
        PlayerPrefs.Save();

        string sceneName = currLevelData.planetName + currLevelData.levelID;

        LoadingScreenManager.Instance.LoadScene(sceneName);
    }

    public void DisplayLevelInfo(LevelData levelData, SpriteRenderer fillRenderer)
    {
        currLevelData = levelData;

        levelDescription.text = levelData.description;
        levelID.text = levelData.levelID.ToString();
        levelTitle.text = levelData.title;

        intialColor = fillRenderer.material.color;
        fillRenderer.material.color = Color.red;

        this.fillRenderer = fillRenderer;

        if (!levelData.completed)
        {
            levelState.text = "Level State: <color=#F8FF00>Incomplete</color>";
        } 
        else
        {
            levelState.text = "Level State: <color=#8FCE00>Completed</color>";
        }
    }
}
