using UnityEngine;
using UnityEngine.SceneManagement;

public class OnCompleteTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string planetName = PlayerPrefs.GetString("levelPlanet");
        int levelID = PlayerPrefs.GetInt("levelID");

        if (planetName == null || planetName == "" || levelID == 0)
        {
            Debug.LogError($"Level data for the current level not found. " +
                $"You need to enter the level from the planetSelection scene!");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Level ID: {levelID} | Planet: {planetName}");
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
