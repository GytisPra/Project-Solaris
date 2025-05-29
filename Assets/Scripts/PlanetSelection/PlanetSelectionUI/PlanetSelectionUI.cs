using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class PlanetSelectionUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public List<GameObject> planetsList;
    public RectTransform buttons;
    public PlanetsDatabase planetsDatabase;
    public UnlockManager unlockManager;

    private CameraRotation cameraRotation;
    private GameObject unlockedbuttonInstance;
    private GameObject lockedbuttonInstance;
    private Coroutine pollingCoroutine;
    private readonly Dictionary<string, bool> planetUnlockStates = new();

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();

        if (lockedbuttonInstance == null)
            lockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/LockedButton");

        if (unlockedbuttonInstance == null)
            unlockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/UnlockedButton");
    }

    private void OnEnable()
    {
        LoadButtons();
        pollingCoroutine = StartCoroutine(PollForPlanetUnlocks());
    }

    private void OnDisable()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }

        // Clear the buttons when disabeling
        foreach (Transform child in buttons.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void LoadButtons()
    {
        if (lockedbuttonInstance == null)
            lockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/LockedButton");

        if (unlockedbuttonInstance == null)
            unlockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/UnlockedButton");

        if (planetsDatabase != null)
        {
            foreach (Planet planet in planetsDatabase.planets)
            {
                planet.LoadUnlockData();

                CreatePlanetButton(planet);

                planetUnlockStates[planet.planetName] = !planet.IsPlanetPassOver() || planet.isFree;
            }
        }
        else
        {
            Debug.LogError("Planets database not assigned!");
        }
    }
    private GameObject CreatePlanetButton(Planet planet)
    {
        bool unlocked = !planet.IsPlanetPassOver() || planet.isFree;

        var prefab = unlocked ? unlockedbuttonInstance : lockedbuttonInstance;
        GameObject buttonInstance;

        buttonInstance = Instantiate(prefab, buttons.transform, false);
        buttonInstance.name = planet.planetName.Trim();

        buttonInstance.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.Center;
        var button = buttonInstance.GetComponent<Button>();

        if (unlocked)
        {
            int daysLeft = planet.GetDaysLeft();
            string buttonText = $"{planet.planetName}\nDays Left: {daysLeft}";

            if (planet.isFree || daysLeft == int.MaxValue)
            {
                buttonText = planet.planetName;
            }

            buttonInstance.GetComponentInChildren<TMP_Text>().text = buttonText;

            ColorBlock cb = button.colors;
            cb.normalColor = new Color(planet.buttonColor.r, planet.buttonColor.g, planet.buttonColor.b, 0.8f);
            cb.highlightedColor = planet.buttonColor;
            cb.selectedColor = planet.buttonColor;
            cb.pressedColor = planet.buttonColor;
            button.colors = cb;

            GameObject planetGameObject = planetsList.Find(p =>
                  string.Equals(p.name, planet.planetName, StringComparison.OrdinalIgnoreCase));

            if (planetGameObject != null)
            {
                button.onClick.AddListener(() => MoveToPlanet(planetGameObject));
            }
            else
            {
                Debug.LogError($"Planet gameObject for entry with planet name {planet.planetName} not found!");
            }
        }
        else
        {
            buttonInstance.GetComponentInChildren<TMP_Text>().text = planet.planetName;

            button.interactable = false;

            Color darkenedColor = planet.buttonColor * 0.5f;
            darkenedColor.a = 1f;

            ColorBlock cb = button.colors;
            cb.normalColor = new Color(darkenedColor.r, darkenedColor.g, darkenedColor.b, 0.8f);
            cb.highlightedColor = darkenedColor;
            cb.selectedColor = darkenedColor;
            cb.pressedColor = darkenedColor;
            cb.disabledColor = new Color(darkenedColor.r, darkenedColor.g, darkenedColor.b, 0.6f);
            button.colors = cb;
        }

        return buttonInstance;
    }

    public bool UnlockPlanet(string planetName)
    {
        var planet = Array.Find(planetsDatabase.planets, s => s.planetName == planetName);
        bool unlocked = !planet.IsPlanetPassOver();

        if (planet == null)
        {
            Debug.LogWarning($"Planet with the name '{planetName}' not found.");
            return false;
        }

        Transform oldButton = buttons.transform.Find(planet.planetName);
        int siblingIndex = -1;
        if (oldButton != null)
        {
            siblingIndex = oldButton.GetSiblingIndex();
            Destroy(oldButton.gameObject);
        }

        GameObject newButton = CreatePlanetButton(planet);
        if (siblingIndex >= 0) // Make sure the position in the hierarchy is the same as the oldButton
        {
            newButton.transform.SetSiblingIndex(siblingIndex);
        }

        StartCoroutine(DelayedLayoutRebuild());
        return true;
    }

    private IEnumerator PollForPlanetUnlocks()
    {
        while (true)
        {
            yield return unlockManager.FetchAndUnlockPlanets();

            foreach (Planet planet in planetsDatabase.planets)
            {
                planet.LoadUnlockData();

                bool isNowUnlocked = !planet.IsPlanetPassOver() || planet.isFree;

                // Check if state has changed
                if (planetUnlockStates.TryGetValue(planet.planetName, out bool wasUnlocked))
                {
                    if (!wasUnlocked && isNowUnlocked)
                    {
                        Debug.Log($"Planet {planet.planetName} has just been unlocked!");

                        // Call your existing UnlockPlanet method
                        UnlockPlanet(planet.planetName);
                    }

                    // Update state
                    planetUnlockStates[planet.planetName] = isNowUnlocked;
                }
                else
                {
                    // If somehow missing, add it
                    planetUnlockStates[planet.planetName] = isNowUnlocked;
                }
            }

            yield return new WaitForSeconds(5f); // Wait for five seconds before checking again
        }
    }

    public void MoveToPlanet(GameObject planet)
    {
        if (planet != null)
        {
            PlayerPrefs.SetString("LastPlanet", planet.name);
            PlayerPrefs.Save();

            cameraRotation.SetTargetObject(planet);

            if (planetSelectionUIManager != null)
            {
                planetSelectionUIManager.SetTravelUICanvasActive(false);
                planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                planetSelectionUIManager.SetPlanetUICanvasActive(true);
            }
            else
            {
                Debug.LogError("planetSelectionUIManager not assigned in the inspector!");
            }
        }
        else
        {
            Debug.LogError($"Planet not provided!");
        }
    }

    public void MoveToPlanet(string planetName)
    {
        GameObject planet = planetsList.Find(p =>
            string.Equals(p.name, planetName, StringComparison.OrdinalIgnoreCase));

        if (planet != null)
        {
            PlayerPrefs.SetString("LastPlanet", planetName);
            PlayerPrefs.Save();

            cameraRotation.SetTargetObject(planet);
            cameraRotation.ResetCameraOnCurrentTarget();

            if (planetSelectionUIManager != null)
            {
                planetSelectionUIManager.SetMainMenuCanvasActive(false);
                planetSelectionUIManager.SetTravelUICanvasActive(false);
                planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
                planetSelectionUIManager.SetPlanetUICanvasActive(true);
            }
            else
            {
                Debug.LogError("planetSelectionUIManager not assigned in the inspector!");
            }
        }
        else
        {
            Debug.LogError($"Planet with the name {planetName} not found!");
        }
    }

    private IEnumerator DelayedLayoutRebuild()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(buttons);
    }

    public void ReturnToTravelMenu()
    {
        planetSelectionUIManager.ClosePlanetSelectionUI();
    }

    public void ResetSolarSystemCamera()
    {
        cameraRotation.ResetCamera();
    }

    public void GoToWebsite()
    {
        string url;
#if UNITY_EDITOR
        url = "http://localhost:5173/";
#else
        url = "https://project-solaris-shop.vercel.app/";
#endif

        Application.OpenURL(url);
    }
}
