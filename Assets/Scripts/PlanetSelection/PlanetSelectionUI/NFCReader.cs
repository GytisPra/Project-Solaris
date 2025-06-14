using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NFCReader : MonoBehaviour
{
    private bool isScanning = false;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;
    private float dotTimer;
    private int dotCount;
    private readonly string baseMessage = "Waiting for card";

    public TMP_Text waitingText; 
    public string result;
    public GameObject firstFrame;
    public GameObject scanCardPopup;
    public List<GameObject> planets;
    public PlanetsDatabase planetsDatabase;
    public CameraRotation cameraRotation;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public PlanetHider planetHider;

    public void StartScan()
    {
        isScanning = true;

        if (Application.platform == RuntimePlatform.Android)
        {
            var mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            mActivity.Call("setIntent", new AndroidJavaObject("android.content.Intent"));
        }

        Debug.Log("NFC Scan started.");
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
            sAction = mIntent.Call<string>("getAction");

            // Clear any pending NFC intents if not scanning
            if (!isScanning && !string.IsNullOrEmpty(sAction))
            {
                mActivity.Call("setIntent", new AndroidJavaObject("android.content.Intent"));
                return;
            }
        }

        if (isScanning)
        {
            Debug.Log("Waiting for card...");

            firstFrame.SetActive(false);
            scanCardPopup.SetActive(true);

            dotTimer += Time.deltaTime;
            if (dotTimer >= 0.5f)
            {
                dotCount = (dotCount + 1) % 4; // cycles from 0 to 3
                string dots = new('.', dotCount);
                waitingText.text = baseMessage + dots;
                dotTimer = 0f;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                    sAction = mIntent.Call<string>("getAction");

                    if (sAction == "android.nfc.action.NDEF_DISCOVERED")
                    {
                        Debug.Log("Tag of type NDEF detected.");

                        AndroidJavaObject[] rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
                        AndroidJavaObject[] records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");
                        sbyte[] payLoad = records[0].Call<sbyte[]>("getPayload");
                        byte[] payloadBytes = Array.ConvertAll(payLoad, b => (byte)b);

                        int langCodeLength = payloadBytes[0] & 0x3F;
                        result = System.Text.Encoding.UTF8.GetString(payloadBytes, 1 + langCodeLength, payloadBytes.Length - 1 - langCodeLength);

                        sbyte[] tagIdBytes = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG").Call<sbyte[]>("getId");
                        string id = Convert.ToBase64String(Array.ConvertAll(tagIdBytes, b => (byte)b));

                        MoveAndUnlock(result);

                        mActivity.Call("setIntent", new AndroidJavaObject("android.content.Intent"));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("NFC scan error: " + ex.Message);
                }
            }
        }
        else
        {
            firstFrame.SetActive(true);
            scanCardPopup.SetActive(false);
        }
    }

    void MoveAndUnlock(string planetName)
    {
        isScanning = false;

        //if (!UnlockPlanet(planetName))
        //{
        //    Debug.LogError("Error while unlocking planet!");
        //    return;
        //}

        planetHider.UnhidePlanet(planetName);
        scanCardPopup.SetActive(false);
        MoveToPlanet(planetName);
    }

    public void Return()
    {
        isScanning = false;
        firstFrame.SetActive(true);
        scanCardPopup.SetActive(false);
    }

    /// <summary>
    /// Unlocks the planet.
    /// </summary>
    /// <param name="planetName">Name of the planet to unlock.</param>
    /// <returns>True if successfully unlocked; false otherwise.</returns>
    //private bool UnlockPlanet(string planetName)
    //{
    //    var planet = Array.Find(planetsDatabase.planets, s => s.planetName == planetName);

    //    if (planet == null)
    //    {
    //        Debug.LogWarning($"Planet with the name '{planetName}' not found.");
    //        return false;
    //    }

    //    planet.SetPlanetToUnlocked(DateTime.MaxValue);
    //    return true;
    //}

    private void MoveToPlanet(string planetName)
    {
        GameObject planet = planets.Find(p => string.Equals(p.name, planetName, StringComparison.OrdinalIgnoreCase));

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
            Debug.LogError($"Planet with the name {planetName} could not be found!");
        }
    }
}
