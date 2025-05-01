using UnityEngine;

[System.Serializable]
public class Subject
{
    public int ID;
    public bool isUnlocked = false;
    public string title;

    public void UnlockSubject()
    {
        isUnlocked = true;
        PlayerPrefs.SetInt($"SubjectUnlocked_{title.Trim()}", 1);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class SubjectPageBinding
{
    public string subjectTitle;
    public GameObject theoryPage;
}
