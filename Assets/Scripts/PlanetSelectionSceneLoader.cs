using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetSelectionSceneLoader : MonoBehaviour
{
    void Start()
    {
        if(!Utils.IsSceneLoaded("PersistentUI")) {
            SceneManager.LoadScene("PersistentUI", LoadSceneMode.Additive);
        }
    }

}
