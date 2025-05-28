using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LampRotationController : MonoBehaviour
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

    private Transform lampHold;

    [SerializeField] private HoldButton increaseButton;
    [SerializeField] private HoldButton decreaseButton;

    [SerializeField] private IslandMover islandMover;

    private float currentAngle;

    [SerializeField] private Canvas interactionPopup;
    [SerializeField] private Button interactButton;
    [SerializeField] private LevelUIManager levelUIManager;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text currentAngleText;
    [SerializeField] private GameObject completed;
    [SerializeField] private FloatRange answerRange;

    public float minAngle = -45f;
    public float maxAngle = 45f;

    public float lampRotationSpeed = 50f;

    private void Start()
    {
        increaseButton.OnHoldAction = Increase;
        decreaseButton.OnHoldAction = Decrease;
        lampHold = transform.parent.parent;
    }

    public void OpenPopup()
    {
        interactionPopup.gameObject.SetActive(true);
        levelUIManager.SetLevelUICanvasActive(false);

        interactButton.interactable = false;

        currentAngle = Mathf.Round(slider.value);
        currentAngleText.text = $"{Mathf.Abs(currentAngle)}°";
    }

    private void CheckIfAngleIsCorrect()
    {
        if(answerRange.Contains(currentAngle))
        {
            completed.SetActive(true);
            islandMover.StartMoving(2f);
        }
        else
        {
            completed.SetActive(false);
            islandMover.MoveBack(4f);
        }
    }


    public void ClosePopup()
    {
        interactButton.interactable = true;

        interactionPopup.gameObject.SetActive(false);
        levelUIManager.SetLevelUICanvasActive(true);
    }

    public void Increase()
    {
        if (currentAngle + 1 > maxAngle)
        {
            return;
        }

        currentAngle += 1f;

        currentAngleText.text = $"{Mathf.Abs(currentAngle)}°";
        slider.value = Mathf.Round(slider.value + 1f);

        CheckIfAngleIsCorrect();
    }

    public void Decrease()
    {
        if (currentAngle - 1 < minAngle)
        {
            return;
        }

        currentAngle -= 1f;

        currentAngleText.text = $"{Mathf.Abs(currentAngle)}°";
        slider.value = Mathf.Round(slider.value - 1f);

        CheckIfAngleIsCorrect();
    }

    public void OnInputFieldValueChanged()
    {
        var roundedValue = Mathf.Round(slider.value);

        currentAngleText.text = $"{Mathf.Abs(roundedValue)}°";
        currentAngle = roundedValue;

        var targetAngle = (slider.value + 360f) % 360f;

        Vector3 currentEuler = lampHold.localEulerAngles;
        currentEuler.y = targetAngle;

        Quaternion targetRotation = Quaternion.Euler(currentEuler);

        lampHold.localRotation = targetRotation;

        CheckIfAngleIsCorrect();
    }
}
