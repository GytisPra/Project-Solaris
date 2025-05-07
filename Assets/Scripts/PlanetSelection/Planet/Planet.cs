using System;
using UnityEngine;

[System.Serializable]
public class Planet
{
    public int ID;
    public string planetName;
    public Color buttonColor;
    public bool unlocked = false;
    public int daysLeft;

    private DateTime unlockedAt;
    public bool IsPlanetPassOver()
    {
        return (DateTime.Now - unlockedAt).TotalDays > daysLeft;
    }
    public void SetPlanetToUnlocked(int unlockForDays)
    {
        unlockedAt = DateTime.Now;
        unlocked = true;
        daysLeft = unlockForDays;
        PlayerPrefs.SetInt($"{planetName}_{ID}", 1);
        PlayerPrefs.SetString($"{planetName}_{ID}_UnlockedAt", unlockedAt.ToString());
        PlayerPrefs.Save();
    }

    public void SetPlanetToLocked()
    {
        unlocked = false;
        daysLeft = 0;
        PlayerPrefs.SetInt($"{planetName}_{ID}", 0);
        PlayerPrefs.Save();
    }

    public void LoadUnlockData()
    {
        if (PlayerPrefs.HasKey($"{planetName}_{ID}"))
        {
            unlocked = PlayerPrefs.GetInt($"{planetName}_{ID}") == 1;
            if (PlayerPrefs.HasKey($"{planetName}_{ID}_UnlockedAt"))
            {
                DateTime.TryParse(PlayerPrefs.GetString($"{planetName}_{ID}_UnlockedAt"), out unlockedAt);
            }
        }
    }
}