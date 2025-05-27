using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LampRotationController : MonoBehaviour
{
    private Transform lamp;

    [SerializeField] private HoldButton increaseButton;
    [SerializeField] private HoldButton decreaseButton;

    private float rotateTo;

    [SerializeField] private Canvas interactionPopup;
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private AnimateLampRay3 rayAnimationController;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text currentAngle;
    [SerializeField] private TMP_Text errorText;

    public float lampRotationSpeed = 50f;

    private void Start()
    {
        increaseButton.OnHoldAction = Increase;
        decreaseButton.OnHoldAction = Decrease;

        lamp = transform;
    }

    public void OpenPopup()
    {
        interactionPopup.gameObject.SetActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(true);
        levelUIManager.SetLevelUICanvasActive(false);

        rotateTo = Mathf.Round(lamp.localEulerAngles.x);
        currentAngle.text = $"{rotateTo}°";
        inputField.text = "";
        errorText.text = "";
        errorText.gameObject.SetActive(false);

        Debug.Log($"Lamp rotation (Unity Euler X): {rotateTo}°");
    }

    public void ClosePopup()
    {
        interactionPopup.gameObject.SetActive(false);
        levelUIManager.SetDepthOfFieldEffectActive(false);
        levelUIManager.SetLevelUICanvasActive(true);
    }

    public void Confirm()
    {
        errorText.text = "";
        errorText.gameObject.SetActive(false);

        if (!TryParseInputField(out float inputedAngle))
        {
            errorText.text = "Invalid input. Please enter a number.";
            errorText.gameObject.SetActive(true);
            return;
        }

        inputedAngle = Mathf.Round(inputedAngle);

        if (!IsAngleValid(inputedAngle))
        {
            errorText.text = "Angle must be between 315°–360° or 0°–45°";
            errorText.gameObject.SetActive(true);
            return;
        }

        StartCoroutine(RotateLamp(inputedAngle));
        Debug.Log($"Parsed input value: {inputedAngle}");
    }

    public void Increase()
    {
        rotateTo = Mathf.Round((rotateTo + 1f) % 360f);

        if (!IsAngleValid(rotateTo)) return;

        currentAngle.text = $"{rotateTo}°";
        inputField.text = "";
    }

    public void Decrease()
    {
        rotateTo = Mathf.Round((rotateTo - 1f + 360f) % 360f);

        if (!IsAngleValid(rotateTo)) return;

        currentAngle.text = $"{rotateTo}°";
        inputField.text = "";
    }

    public void OnInputFieldValueChanged()
    {
        if (!string.IsNullOrWhiteSpace(inputField.text))
        {
            if (!TryParseInputField(out float parsed))
            {
                errorText.text = "Invalid input.";
                errorText.gameObject.SetActive(true);
                return;
            }

            parsed = Mathf.Round(parsed);

            if (!IsAngleValid(parsed))
            {
                errorText.text = "Angle must be between 315°–360° or 0°–45°";
                errorText.gameObject.SetActive(true);
                return;
            }

            errorText.text = "";
            errorText.gameObject.SetActive(false);
            currentAngle.text = $"{parsed}°";
        }
        else
        {
            currentAngle.text = $"{Mathf.Round(rotateTo)}°";
            errorText.text = "";
            errorText.gameObject.SetActive(false);
        }
    }

    private bool IsAngleValid(float angle)
    {
        angle = (angle + 360f) % 360f; // Normalize

        if (angle >= 315f && angle <= 360f)
        {
            return true;
        }

        if (angle <= 45f && angle >= 0f)
        {
            return true;
        }

        return false;
    }

    private IEnumerator RotateLamp(float targetAngle)
    {
        ClosePopup();

        yield return StartCoroutine(rayAnimationController.HideRays());
        
        Vector3 currentEuler = lamp.localEulerAngles;
        currentEuler.x = targetAngle;

        Quaternion targetRotation = Quaternion.Euler(currentEuler);

        Debug.Log($"Rotating lamp to {targetAngle}° (Unity X)");

        while (Quaternion.Angle(lamp.localRotation, targetRotation) > 0.1f)
        {
            lamp.localRotation = Quaternion.RotateTowards(
                lamp.localRotation,
                targetRotation,
                Time.deltaTime * lampRotationSpeed
            );

            yield return null;
        }

        lamp.localRotation = targetRotation;
        StartCoroutine(rayAnimationController.RevealRays());
    }

    private bool TryParseInputField(out float inputedAngle)
    {
        Debug.Log($"Input text: {inputField.text}");

        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            inputedAngle = 0;
            return false;
        }

        return float.TryParse(inputField.text, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out inputedAngle);
    }
}
