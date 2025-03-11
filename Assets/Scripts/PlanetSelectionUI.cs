using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSelectionUI : MonoBehaviour
{
    private string lastPlanet;


    public void Start()
    {
        lastPlanet = PlayerPrefs.GetString("LastPlanet", "Earth");
    }
    public void LoadEarth()
    {
        SceneManager.LoadScene("Earth");
    }

    public void LoadMars()
    {
        SceneManager.LoadScene("Mars");
    }

    public void LoadJupiter()
    {
        SceneManager.LoadScene("Jupiter");
    }

    public void LoadVenus()
    {
        SceneManager.LoadScene("Venus");
    }

    public void Back()
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
}
