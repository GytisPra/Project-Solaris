using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class PlanetSelectionUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public Transform planets;
    public List<GameObject> planetsList;
    public RectTransform buttons;
    public PlanetsDatabase planetsDatabase;

    private CameraRotation cameraRotation;
    private GameObject unlockedbuttonInstance;
    private GameObject lockedbuttonInstance;

    private void OnEnable()
    {
        StartCoroutine(FetchUnlockedPlanets());
    }

    private IEnumerator FetchUnlockedPlanets()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://project-solaris-shop-production.up.railway.app/unlocks");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Unlocked planets: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error fetching unlocks: " + request.error);
        }
    }

    void Start()
    {
        cameraRotation = Camera.main.GetComponent<CameraRotation>();

        lockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/LockedButton");
        unlockedbuttonInstance = Resources.Load<GameObject>("UI/PlanetSelection/UnlockedButton");

        StartCoroutine(FetchUnlockedPlanets());

        LoadButtons();
    }

    private void LoadButtons()
    {
        if (planetsDatabase != null)
        {
            foreach (Planet planet in planetsDatabase.planets)
            {
                planet.LoadUnlockData();

                if (planet.IsPlanetPassOver())
                {
                    planet.SetPlanetToLocked();
                }

                CreatePlanetButton(planet);
            }

            //StartCoroutine(DelayedLayoutRebuild());
        }
        else
        {
            Debug.LogError("Planets database not assigned!");
        }
    }
    private GameObject CreatePlanetButton(Planet planet)
    {
        var prefab = planet.unlocked ? unlockedbuttonInstance : lockedbuttonInstance;
        GameObject buttonInstance;

        buttonInstance = Instantiate(prefab, buttons.transform, false);
        buttonInstance.name = planet.planetName.Trim();

        buttonInstance.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.Center;
        var button = buttonInstance.GetComponent<Button>();

        if (planet.unlocked)
        {
            buttonInstance.GetComponentInChildren<TMP_Text>().text = $"{planet.planetName}\nDays Left: {planet.daysLeft}";

            ColorBlock cb = button.colors;
            cb.normalColor = new Color(planet.buttonColor.r, planet.buttonColor.g, planet.buttonColor.b, 0.4f);
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
            cb.normalColor = new Color(darkenedColor.r, darkenedColor.g, darkenedColor.b, 0.4f);
            cb.highlightedColor = darkenedColor;
            cb.selectedColor = darkenedColor;
            cb.pressedColor = darkenedColor;
            cb.disabledColor = new Color(darkenedColor.r, darkenedColor.g, darkenedColor.b, 0.4f);
            button.colors = cb;
        }

        //StartCoroutine(DelayedLayoutRebuild());

        return buttonInstance;
    }

    public bool UnlockPlanet(string planetName, int days)
    {
        var planet = Array.Find(planetsDatabase.planets, s => s.planetName == planetName);

        if (planet == null)
        {
            Debug.LogWarning($"Planet with the name '{planetName}' not found.");
            return false;
        }

        if (planet.unlocked)
        {
            Debug.Log($"Subject '{planetName}' is already unlocked.");
            return false;
        }
        planet.SetPlanetToUnlocked(days);



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

        //StartCoroutine(DelayedLayoutRebuild());

        return true;
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

        GameObject planet = planets.Find(planetName).gameObject;

        if (planet != null)
        {
            PlayerPrefs.SetString("LastPlanet", planetName);
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

    private IEnumerator DelayedLayoutRebuild()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(buttons);
    }

    public void ReloadButtons()
    {
        foreach (Transform child in buttons.transform)
        {
            Destroy(child.gameObject);
        }

        LoadButtons();
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
        Application.OpenURL("https://project-solaris-shop-production.up.railway.app/");
    }
}
