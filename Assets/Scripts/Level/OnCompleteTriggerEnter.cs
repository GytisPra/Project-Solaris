using UnityEngine;
using UnityEngine.SceneManagement;

public class OnCompleteTriggerEnter : MonoBehaviour
{
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            string planetName = PlayerPrefs.GetString("levelPlanet");
            int levelID = PlayerPrefs.GetInt("levelID");

            if (string.IsNullOrEmpty(planetName) || levelID == 0)
            {
                Debug.LogError("Level data not found! Enter from planet selection scene.");
                return;
            }

            Debug.Log($"Level ID: {levelID} | Planet: {planetName}");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("PlanetSelection");
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
