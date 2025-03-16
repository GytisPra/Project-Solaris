using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public TextMeshProUGUI levelInfo;
    public TextAsset[] jsonFiles;

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetLevelUICanvasActive(false);
        planetSelectionUIManager.SetPlanetUICanvasActive(true);
    }

    public void DisplayLevelInfo(int levelID)
    {
        foreach (var jsonFile in jsonFiles)
        {
            Levels levelsInJson = JsonUtility.FromJson<Levels>(jsonFile.text);
            foreach (Level level in levelsInJson.levels)
            {
                if (level.ID == levelID) {
                    levelInfo.text = $"LevelID: {level.ID}, longitude: {level.longitude}, latitude: {level.latitude}, type: {level.type}";
                }
            }
        }
    }
}
