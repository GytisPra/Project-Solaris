using UnityEngine;

[System.Serializable]
public class Level
{
    public int ID;
    public string title;
    public string planet;
    public float latitude;
    public float longitude;
    public string description;
    public bool completed;

    public void SetLevelToCompleted()
    {
        completed = true;
        PlayerPrefs.SetInt($"{planet}_{title}_{ID}", 1);
        PlayerPrefs.Save();
    }
}