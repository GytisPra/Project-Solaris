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

    public static float GetSphereRadius(GameObject planet)
    {
        if (planet == null)
        {
            Debug.LogError("Sphere GameObject is null.");
            return 0f;
        }

        return planet.name switch
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
            _ => throw new System.Exception($"Could not find radius for {planet.name}. Check Utils!"),
        };
    }
}