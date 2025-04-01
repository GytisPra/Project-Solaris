using UnityEngine;
using UnityEngine.SceneManagement;

public class OnCompleteTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        LevelData levelData = LevelDataHolder.currLevelData;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Level ID: {levelData.levelID} | Planet: {levelData.planetName}");
            //TextAsset levelJson = Utils.GetJsonFile(levelData.planetName);

            SceneManager.LoadScene("PlanetSelection");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlanetSelection")
        {
            MainMenuScript mainMenuScript = FindAnyObjectByType<MainMenuScript>();
            if (mainMenuScript != null)
            {
                mainMenuScript.StartGame();
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
