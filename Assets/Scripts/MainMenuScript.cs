using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private string lastPlanet;

    void Start()
    {
        if (!PlayerPrefs.HasKey("LastPlanet"))
        {
            PlayerPrefs.SetString("LastPlanet", "Earth");
            PlayerPrefs.Save();
            lastPlanet = "Earth";
        }
        else
        {
            lastPlanet = PlayerPrefs.GetString("LastPlanet", "Earth");
        }

        if (!Utils.IsSceneLoaded("PersistentUI"))
        {
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        }
    }
    public void StartGame()
    {
        if (lastPlanet.Equals("MainMenuScene"))
        {
            SceneManager.LoadScene("Earth");
        }
        else
        {
            SceneManager.LoadScene(lastPlanet);
        }
    }

    public void OpenOptions()
    {
        Debug.Log("Opening Options");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
