using System;
using UnityEngine;

[System.Serializable]
public class Planet
{
    public int ID;
    public string planetName;
    public Color buttonColor;
    public bool isFree = false;
    public DateTime unlockedUntil;

    public bool IsPlanetPassOver()
    {
        if (unlockedUntil == DateTime.MinValue)
            return true;

        if (unlockedUntil == DateTime.MaxValue)
            return false; // never expires

        return DateTime.Now > unlockedUntil;
    }

    public int GetDaysLeft()
    {
        if (unlockedUntil == DateTime.MaxValue)
        {
            return int.MaxValue;
        }
            

        return Math.Max(0, (int)(unlockedUntil - DateTime.UtcNow).TotalDays);
    }

    public bool IsAlreadyUnlockedAt(DateTime date)
    {
        var normalizedStored = unlockedUntil.ToUniversalTime();
        var normalizedIncoming = date.ToUniversalTime();

        return Math.Abs((normalizedStored - normalizedIncoming).TotalSeconds) < 1;
    }

    public void SetPlanetToUnlocked(DateTime newUnlockedUntil)
    {
        if (newUnlockedUntil == DateTime.MaxValue)
        {
            unlockedUntil = DateTime.MaxValue;
        } 
        else
        {
            unlockedUntil = newUnlockedUntil.ToUniversalTime();
        }

        PlayerPrefs.SetInt($"{planetName}_{ID}_Unlocked", 1);
        PlayerPrefs.SetString($"{planetName}_{ID}_UnlockedUntil", unlockedUntil.ToString("o")); // use ISO format
        PlayerPrefs.Save();
    }

    public void SetPlanetToUnlockedNoPrefs(DateTime newUnlockedUntil)
    {
        if (newUnlockedUntil == DateTime.MaxValue)
            unlockedUntil = DateTime.MaxValue;
        else
            unlockedUntil = newUnlockedUntil.ToUniversalTime();
    }

    public void SetPlanetToLocked()
    {
        unlockedUntil = DateTime.MinValue;
        PlayerPrefs.SetInt($"{planetName}_{ID}_Unlocked", 0);
        PlayerPrefs.SetString($"{planetName}_{ID}_UnlockedUntil", DateTime.MinValue.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads data from player perfs. Locks the planet if the pass has expired
    /// </summary>
    public void LoadUnlockData()
    {
        if (PlayerPrefs.HasKey($"{planetName}_{ID}_Unlocked"))
        {
            DateTime.TryParse(
                PlayerPrefs.GetString($"{planetName}_{ID}_UnlockedUntil"), 
                    null, 
                    System.Globalization.DateTimeStyles.RoundtripKind, 
                    out unlockedUntil
                );

            // Check if the pass has expired
            if (IsPlanetPassOver())
            {
                SetPlanetToLocked();
            }
        }
    }
}