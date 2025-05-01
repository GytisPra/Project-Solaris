using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NFCReader : MonoBehaviour
{
    public bool tagFound = false;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;
    [SerializeField] private NFCManager nfcManager;
    public GameObject scannedPlanet;
    public PlanetSelectionUI planetselectionUI;
    public TextMeshProUGUI planet;
    public TextMeshProUGUI cardScanner;
    [SerializeField] public string result;
    public Button button;

    void Update()
    {
       
        if (nfcManager.AllowedNFC == true)
        {
            
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!tagFound)
                {
                    try
                    {

                        // Create new NFC Android object
                        mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                        mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                        sAction = mIntent.Call<String>("getAction");
                        if (sAction == "android.nfc.action.NDEF_DISCOVERED")
                        {

                            Debug.Log("Tag of type NDEF");
                            AndroidJavaObject[] rawMsg = mIntent.Call<AndroidJavaObject[]>("getParcelableArrayExtra", "android.nfc.extra.NDEF_MESSAGES");
                            AndroidJavaObject[] records = rawMsg[0].Call<AndroidJavaObject[]>("getRecords");
                            byte[] payLoad = records[0].Call<byte[]>("getPayload");
                            int langCodeLength = payLoad[0] & 0x3F;
                            //string result = System.Text.Encoding.Default.GetString(payLoad);
                            result = System.Text.Encoding.UTF8.GetString(payLoad, 1 + langCodeLength, payLoad.Length - 1 - langCodeLength);

                            planet.text = "Go to " + result;

                            byte[] payLoad2 = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG").Call<byte[]>("getId");
                            string text = System.Convert.ToBase64String(payLoad2);
                            string id = text;

                            ShowButton(result);

                            

                            tagFound = true;

                            mActivity.Call("setIntent", new AndroidJavaObject("android.content.Intent"));
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = ex.Message;

                    }
                }
            }
        }
    }

    void ShowButton(string planetName)
    {
        scannedPlanet.GetComponent<Button>().onClick.AddListener(() => planetselectionUI.MoveToPlanet(planetName));
        scannedPlanet.SetActive(true);
    }

    public void DisableButton()
    {
        tagFound = false;
        scannedPlanet.SetActive(false);
    }

    public void CardScanning()
    {
        if (nfcManager.AllowedNFC == false)
        {
            cardScanner.text = "Card scanning is disabled";
        }
        else if (nfcManager.AllowedNFC == true)
        {
            cardScanner.text = "Card scanning is enabled";
        }
    }
}
