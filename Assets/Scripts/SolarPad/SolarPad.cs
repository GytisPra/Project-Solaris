using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolarPad : MonoBehaviour
{
    public static SolarPad Instance { get; private set; }
    public GameObject Subjects;
    public List<SubjectPageBinding> subjectPageBindings;
    public SubjectsDatabase subjectsDatabase;

    private Camera solarPadCam;
    private Canvas solarPadCanvas;
    private bool isTheoryOpen;
    private GameObject currentTheoryPage;
    private GameObject unlockedButtonPrefab;
    private GameObject lockedButtonPrefab;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        solarPadCanvas = GetComponentInChildren<Canvas>();
        solarPadCam = GetComponentInChildren<Camera>();

        solarPadCam.gameObject.SetActive(false);
        solarPadCanvas.gameObject.SetActive(false);

        lockedButtonPrefab = Resources.Load<GameObject>("UI/SolarPad/LockedButton");
        unlockedButtonPrefab = Resources.Load<GameObject>("UI/SolarPad/UnlockedButton");

        foreach (Transform child in Subjects.transform)
        {
            Destroy(child.gameObject);
        }

        LoadSubjects();
    }

    private void LoadSubjects()
    {
        if (subjectsDatabase != null)
        {
            foreach (Subject subject in subjectsDatabase.subjects)
            {
                if (PlayerPrefs.HasKey($"SubjectUnlocked_{subject.title.Trim()}"))
                {
                    subject.isUnlocked = true;
                }

                CreateSubjectButton(subject);
            }
        }
        else
        {
            Debug.LogError("Subjects json file not found");
        }
    }
    private GameObject CreateSubjectButton(Subject subject)
    {
        var prefab = subject.isUnlocked ? unlockedButtonPrefab : lockedButtonPrefab;
        var button = Instantiate(prefab, Subjects.transform, false);
        button.name = subject.title.Trim();

        if (subject.isUnlocked)
        {
            button.GetComponentInChildren<TMP_Text>().text = subject.title;
            var theoryPage = subjectPageBindings.Find(b => 
                  string.Equals(b.subjectTitle, subject.title, StringComparison.OrdinalIgnoreCase))?.theoryPage;


            if (theoryPage != null)
            {
                button.GetComponent<Button>().onClick.AddListener(() => OpenTheory(theoryPage));
            }
            else
            {
                Debug.LogWarning($"Theory page for subject {subject.title} not found!" +
                    $"\nWill be setting the theory page to DoesNotExist. " +
                    $"Check the Subject page bindings in the inspector.");

                theoryPage = subjectPageBindings.Find(b =>
                    string.Equals(b.subjectTitle, "NotFound", StringComparison.OrdinalIgnoreCase))?.theoryPage;
                button.GetComponent<Button>().onClick.AddListener(() => OpenTheory(theoryPage));
            }
        }

        return button;
    }

    public void Open()
    {
        if (solarPadCam == null)
        {
            Debug.LogError("Cannot open solar pad. No camera found!");
            return;
        }

        solarPadCam.gameObject.SetActive(true);
        solarPadCanvas.gameObject.SetActive(true);

        solarPadCam.depth = 10;

        GameStateManager.Instance.SetState(GameState.Menu);
    }

    public void Close()
    {
        if (isTheoryOpen)
        {
            if (currentTheoryPage != null)
            {
                currentTheoryPage.SetActive(false);
            }
            Subjects.SetActive(true);
            currentTheoryPage = null;
            isTheoryOpen = false;
            return;
        }

        solarPadCam.depth = -10;

        solarPadCam.gameObject.SetActive(false);
        solarPadCanvas.gameObject.SetActive(false);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
    /// <summary>
    /// Unlocks the subject and updates the subjects in the solar pad to show the unlocked button
    /// </summary>
    /// <param name="subjectTitleToUnlock"></param>
    /// <returns>false if the subject is not found or is already unlocked true otherwise</returns>
    public bool UnlockSubject(string subjectTitleToUnlock)
    {
        var subject = Array.Find(subjectsDatabase.subjects, s => s.title == subjectTitleToUnlock);

        if (subject == null)
        {
            Debug.LogWarning($"Subject '{subjectTitleToUnlock}' not found.");
            return false;
        }

        if (subject.isUnlocked)
        {
            Debug.Log($"Subject '{subjectTitleToUnlock}' is already unlocked.");
            return false;
        }
        subject.UnlockSubject();

        Transform oldButton = Subjects.transform.Find(subject.title.Trim());
        int siblingIndex = -1;

        if (oldButton != null)
        {
            siblingIndex = oldButton.GetSiblingIndex();
            Destroy(oldButton.gameObject);
        }

        GameObject newButton = CreateSubjectButton(subject);
        if (siblingIndex >= 0) // Make sure the position in the hierarchy is the same as the oldButton
        {
            newButton.transform.SetSiblingIndex(siblingIndex);
        }

        return true;
    }

    public void OpenTheory(GameObject theoryPage)
    {
        Subjects.SetActive(false);
        theoryPage.SetActive(true);
        currentTheoryPage = theoryPage;
        isTheoryOpen = true;
    }
}

