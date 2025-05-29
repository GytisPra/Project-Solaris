using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LampController : MonoBehaviour
{
    [System.Serializable]
    public class FloatRange
    {
        public float min;
        public float max;

        public bool Contains(float value)
        {
            return value >= min && value <= max;
        }
    }

    [SerializeField] private List<GameObject> lamps;
    [SerializeField] private List<GameObject> completedImages;
    [SerializeField] private List<Button> interactButtons;
    [SerializeField] private List<FloatRange> answers;

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text error;
    [SerializeField] private TMP_Text currentWavelength;
    [SerializeField] private Canvas interactionPopup;
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private HoldButton increaseButton;
    [SerializeField] private HoldButton decreaseButton;
    [SerializeField] private GateOpen gateOpen;
    [SerializeField] private int requiredCorrectCount = 3;


    private AnimateLampRay2 rayAnimator;
    private int lampIndex = -1;
    private int correctCount = 0;
    private InputSource lastInputSource = InputSource.None;

    public float maxWavelength = 750f;
    public float minWavelength = 380f;
    private enum InputSource
    {
        None,
        InputField,
        Button
    }

    private void Start()
    {
        increaseButton.OnHoldAction = Increase;
        decreaseButton.OnHoldAction = Decrease;
    }

    public void OpenPopup(int newLampIndex)
    {
        if (newLampIndex < 0 || newLampIndex >= lamps.Count)
        {
            Debug.LogError($"Lamp index {newLampIndex} out of bounds!");
            return;
        }

        var lamp = lamps[newLampIndex];

        if (!lamp.TryGetComponent<AnimateLampRay2>(out rayAnimator))
        {
            Debug.LogError($"Lamp with index {newLampIndex} does not have AnimateLampRay2 component!");
            return;
        }

        lampIndex = newLampIndex;

        currentWavelength.text = $"{rayAnimator.wavelength}nm";

        GameStateManager.Instance.SetState(GameState.Menu);

        inputField.text = "";

        interactionPopup.gameObject.SetActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(true);
        levelUIManager.SetLevelUICanvasActive(false);
    }

    public void ClosePopup()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);

        interactionPopup.gameObject.SetActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);
        levelUIManager.SetLevelUICanvasActive(true);
    }

    public void Confirm()
    {
        if (lampIndex == -1)
        {
            Debug.LogError("Lamp index was not set! SHOULD NOT HAPPEN!");
            ClosePopup();
            return;
        }

        StartCoroutine(ChangeColor());
    }

    public void Increase()
    {
        lastInputSource = InputSource.Button;

        if (!TryParseWavelength(currentWavelength.text, out var currWavelength))
        {
            error.text = $"Failed to parse currentWavelength text!";
            error.gameObject.SetActive(true);

            return;
        }

        if (currWavelength + 1 > maxWavelength)
        {
            return;
        }

        currWavelength += 1f;
        currentWavelength.text = $"{currWavelength}nm";
    }

    public void Decrease()
    {
        lastInputSource = InputSource.Button;

        if (!TryParseWavelength(currentWavelength.text, out var currWavelength))
        {
            error.text = $"Failed to parse currentWavelength text!";
            error.gameObject.SetActive(true);

            return;
        }

        if (currWavelength - 1 < minWavelength)
        {
            return;
        }

        currWavelength -= 1f;
        currentWavelength.text = $"{currWavelength}nm";
    }

    public void OnInputFieldValueChanged()
    {
        lastInputSource = InputSource.InputField;

        if (!string.IsNullOrWhiteSpace(inputField.text))
        {
            if (!TryParseInputField(out var inputedWavelength))
            {
                currentWavelength.text = $"{rayAnimator.wavelength}nm";
                return;
            }

            inputedWavelength = Mathf.Round(inputedWavelength);

            if (inputedWavelength > maxWavelength)
            {
                currentWavelength.text = $"{rayAnimator.wavelength}nm";
                return;
            }

            if (inputedWavelength < minWavelength)
            {
                currentWavelength.text = $"{rayAnimator.wavelength}nm";
                return;
            }

            currentWavelength.text = $"{inputedWavelength}nm";
        }
        else
        {
            currentWavelength.text = $"{rayAnimator.wavelength}nm";
        }
    }

    public IEnumerator ChangeColor()
    {
        int currLampIndex = lampIndex;

        error.text = "";
        error.gameObject.SetActive(false);

        float targetWavelength;

        if (lastInputSource == InputSource.InputField)
        {
            if (!TryParseInputField(out targetWavelength))
            {
                error.text = $"Failed to parse inputed text!";
                error.gameObject.SetActive(true);
                yield break;
            }
        }
        else if (lastInputSource == InputSource.Button)
        {
            string text = currentWavelength.text.Replace("nm", "");
            if (!float.TryParse(text, out targetWavelength))
            {
                error.text = $"Failed to parse currentWavelength text!";
                error.gameObject.SetActive(true);
                yield break;
            }
        }
        else
        {
            Debug.LogWarning("No inputs were made!");
            error.gameObject.SetActive(true);
            yield break;
        }

        // Validate range
        targetWavelength = Mathf.Round(targetWavelength);
        if (targetWavelength > maxWavelength)
        {
            error.text = $"Wavelength must be less than or equal to {maxWavelength}";
            error.gameObject.SetActive(true);
            yield break;
        }

        if (targetWavelength < minWavelength)
        {
            error.text = $"Wavelength must be more than or equal to {minWavelength}";
            error.gameObject.SetActive(true);
            yield break;
        }

        ClosePopup();


        interactButtons[currLampIndex].interactable = false;

        yield return StartCoroutine(rayAnimator.EnableLamp());
        yield return StartCoroutine(rayAnimator.ChangeColor(targetWavelength));

        CheckIsCorrect(targetWavelength, currLampIndex);
    }

    private void CheckIsCorrect(float targetWavelength, int currLampIndex)
    {
        FloatRange range = answers[currLampIndex];

        if (range.Contains(targetWavelength))
        {
            interactButtons[currLampIndex].gameObject.SetActive(false);
            completedImages[currLampIndex].SetActive(true);

            correctCount++;

            if (correctCount == requiredCorrectCount)
            {
                StartCoroutine(gateOpen.Open());
            }
        }
        else
        {
            interactButtons[currLampIndex].interactable = true;
        }
    }

    private bool TryParseWavelength(string text, out float wavelength)
    {
        text = text.Replace("nm", "").Trim();
        return float.TryParse(text, System.Globalization.NumberStyles.Float,
                              System.Globalization.CultureInfo.InvariantCulture,
                              out wavelength);
    }

    private bool TryParseInputField(out float inputedWavelength)
    {
        return float.TryParse(inputField.text, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out inputedWavelength);
    }
}
