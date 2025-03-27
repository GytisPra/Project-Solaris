using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSceneLoader : MonoBehaviour
{
    void Start()
    {
        if (!Utils.IsSceneLoaded("PlaneUI")) {
            SceneManager.LoadScene("PlanetUI", LoadSceneMode.Additive);
        }

        if(!Utils.IsSceneLoaded("PersistentUI")) {
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        }
    }
}
