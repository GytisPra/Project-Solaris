using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnCompleteTriggerEnter : MonoBehaviour
{
    public LevelsDatabase levelsDatabase;
    public PlanetsDatabase planetsDatabase;
    public LevelUIManager levelUIManager;
    string planetName;
    private bool isInTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isInTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            isInTrigger = true;

            levelUIManager.SetLevelUICanvasActive(false);

            levelUIManager.SetFinishLevelPopupCanvasActive(true);
            levelUIManager.SetDepthOfFieldEffectActive(true);

            GameStateManager.Instance.SetState(GameState.Menu);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
        }
    }

    public void ClosePopup()
    {
        levelUIManager.SetLevelUICanvasActive(true);

        levelUIManager.SetFinishLevelPopupCanvasActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void FinishLevel()
    {
        levelUIManager.SetFinishLevelPopupCanvasActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);

        planetName = PlayerPrefs.GetString("levelPlanet");
        int levelID = PlayerPrefs.GetInt("levelID");

        if (string.IsNullOrEmpty(planetName) || levelID == 0)
        {
            Debug.LogError("Level data not found! Enter from planet selection scene.");
            return;
        }

        // find and update the status of the level
        var level = Array.Find(levelsDatabase.levels, l => levelID == l.ID);
        var planet = Array.Find(planetsDatabase.planets, p => p.planetName == planetName);

        if (level == null)
        {
            Debug.LogError("The level was not found!");
        }
        else if (level.completed)
        {
            Debug.LogWarning("The level is already completed");
        }
        else
        {
            planet.IncreaseCompletedLevelsCount();
            level.SetLevelToCompleted();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadingScreenManager.Instance.LoadScene("PlanetSelection");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlanetSelection")
        {
            CameraRotation cameraRotation = FindAnyObjectByType<CameraRotation>();
            MainMenuScript mainMenuScript = FindAnyObjectByType<MainMenuScript>();
         
            if (mainMenuScript != null && cameraRotation != null)
            {
                mainMenuScript.StartGame();
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
