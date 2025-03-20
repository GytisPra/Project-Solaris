using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    public static float GetRadius(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("Sphere is null.");
            return 0f;
        }

        if (obj.name.Contains("Level"))
        {
            return 0.1f;
        }

        return obj.name switch
        {
            "Mercury" => 1f,
            "Venus" => 2f,
            "Earth" => 3f,
            "Mars" => 2.5f,
            "Jupiter" => 6f,
            "Saturn" => 5f,
            "SaturnST" => 5f,
            "Uranus" => 5f,
            "Neptune" => 5f,
            _ => throw new System.Exception($"Could not find radius for {obj.name}. Check Utils!"),
        };
    }
}