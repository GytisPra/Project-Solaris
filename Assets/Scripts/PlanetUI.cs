using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetUI : MonoBehaviour
{
    private string currentPlanetName;

    void Start()
    {
        currentPlanetName = SceneManager.GetActiveScene().name;

        if(!Utils.IsSceneLoaded("PersistentUI")) {
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        }
    }

    private void SaveLastPlanet()
    {
        currentPlanetName = SceneManager.GetActiveScene().name;

        if (!currentPlanetName.Equals("MainMenuScene"))
        {
            PlayerPrefs.SetString("LastPlanet", currentPlanetName);
            PlayerPrefs.Save();
        }
    }

    public void Back()
    {
        SaveLastPlanet();

        SceneManager.LoadScene("MainMenuScene");
        SceneManager.UnloadSceneAsync(currentPlanetName);
    }

    public void ViewAllPlanets()
    {
        SaveLastPlanet();

        SceneManager.LoadScene("PlanetSelection");
    }
}
