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
            "Mercury" => 0.5f,
            "Venus" => 0.8f,
            "Earth" => 1f,
            "Mars" => 0.625f,
            "Jupiter" => 3.15f,
            "Saturn" => 3f,
            "Uranus" => 1.8f,
            "Neptune" => 1.8f,
            _ => throw new System.Exception($"Could not find radius for {obj.name}. Check Utils!"),
        };
    }

    public static float GetRadius(string name)
    {
        return name switch
        {
            "Mercury" => 0.5f,
            "Venus" => 0.8f,
            "Earth" => 1f,
            "Mars" => 0.625f,
            "Jupiter" => 3.15f,
            "Saturn" => 3f,
            "saturn" => 3f,
            "Uranus" => 1.8f,
            "Neptune" => 1.8f,
            _ => throw new System.Exception($"Could not find radius for {name}. Check Utils!"),
        };
    }

    public static TextAsset GetJsonFile(string planetName)
    {
        return Resources.Load<TextAsset>($"{planetName.ToLower()}Levels");
    }
}