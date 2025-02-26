using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
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

    public void Back()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
