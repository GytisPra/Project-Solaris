using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnCompleteTriggerEnter : MonoBehaviour
{
    private bool triggered = false;
    public LevelsDatabase levelsDatabase;
    [SerializeField] private Slider slider;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if(slider.value == 5)
            {
                SceneManager.LoadScene("PlanetSelection");
            }
            
            
            //string planetName = PlayerPrefs.GetString("levelPlanet");
            //int levelID = PlayerPrefs.GetInt("levelID");

            //if (string.IsNullOrEmpty(planetName) || levelID == 0)
            //{
            //    Debug.LogError("Level data not found! Enter from planet selection scene.");
            //    return;
            //}

            //// find and update the status of the level
            //var level = Array.Find(levelsDatabase.levels, l => levelID == l.ID);

            //if (level == null)
            //{
            //    Debug.LogError("The level was not found!");
            //}
            //else if (level.completed)
            //{
            //    Debug.LogWarning("The level is already completed");
            //}
            //else
            //{
            //    level.SetLevelToCompleted();
            //}  

            //SceneManager.sceneLoaded -= OnSceneLoaded;
            //SceneManager.sceneLoaded += OnSceneLoaded;
            
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
