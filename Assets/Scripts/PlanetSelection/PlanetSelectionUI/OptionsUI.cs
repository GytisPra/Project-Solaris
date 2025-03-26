using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public PlanetSelectionUIManager planetSelectionUIManager;
    public TextMeshProUGUI volumeLevel;
    public Slider volumeSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("VolumeLevel"))
        {
            string volumeLevel = PlayerPrefs.GetString("VolumeLevel");

            Debug.Log($"Current volume level: {volumeLevel}");

            this.volumeLevel.text = $"{volumeLevel}%";
            volumeSlider.value = float.Parse(volumeLevel) / 100;
        }
        else
        {
            volumeLevel.text = "0%";
            volumeSlider.value = 0f;
        }
    }

    public void ClosePopUp()
    {
        planetSelectionUIManager.SetOptionsCanvasActive(false);
        planetSelectionUIManager.SetMainMenuCanvasActive(true);
    }

    public void ConfirmSettings()
    {
        PlayerPrefs.SetString("VolumeLevel", Mathf.RoundToInt(volumeSlider.value * 100).ToString());
        PlayerPrefs.Save();

        ClosePopUp();
    }

    public void ResetOptions()
    {
        volumeSlider.value = 0;
        volumeLevel.text = "0%";
    }

    public void UpdateVolumeLevel()
    {
        volumeLevel.text = $"{Mathf.RoundToInt(volumeSlider.value * 100)}%";
    }
}
