using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NFCReader : MonoBehaviour
{
    public bool tagFound = false;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;
    [SerializeField] private NFCManager nfcManager;
    public TextMeshProUGUI planet;

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
                            string result = System.Text.Encoding.Default.GetString(payLoad).Remove(1,2).Trim();
                            planet.text = result;

                            byte[] payLoad2 = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG").Call<byte[]>("getId");
                            string text = System.Convert.ToBase64String(payLoad2);
                            string id = text;

                        }
                    }
                    catch (Exception ex)
                    {
                        string text = ex.Message;
                    }
                }
            }
        }
    }
}
