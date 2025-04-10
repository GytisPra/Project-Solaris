using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionUIScript : MonoBehaviour
{
    public float maxDistance;
    public float minDistance;
    public float moveSpeed = 2f;
    public float targetDistance = 0.5f;
    public LevelUIManager levelUIManager;

    [SerializeField] private Transform lensTransform;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text errorMsg;
    [SerializeField] private AnimateLampRay animateLampRay;
    [SerializeField] private IceMelt iceMelt;

    public void Close()
    {
        levelUIManager.SetInteractionPopupActive(false);
        levelUIManager.SetLevelUICanvasActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(false);
        levelUIManager.SetAboveCharCanvasActive(true);
    }

    public void Confirm()
    {
        bool distanceCorrect = false;
        string inputedText = inputField.text.Replace('.', ',');

        if (ValidateInput(inputedText, out float distance))
        {
            if (distance == targetDistance)
            {
                distanceCorrect = true;
            }

            Vector3 targetPos = new(lensTransform.localPosition.x, lensTransform.localPosition.y, -distance);

            levelUIManager.SetInteractionPopupActive(false);
            levelUIManager.SetLevelUICanvasActive(true);
            levelUIManager.SetDepthOfFieldEffectActive(false);
            levelUIManager.SetAboveCharCanvasActive(true);

            StartCoroutine(MoveLens(targetPos, distanceCorrect));
        }
    }
    public void OnValueChanged()
    {
        animateLampRay.DisableLamp();
    }

    private bool ValidateInput(string input, out float number)
    {
        errorMsg.text = "";

        number = 0;

        if (input.Trim().Length <= 0)
        {
            errorMsg.text = $"Enter a number between {maxDistance} and {minDistance}";
            return false;
        }

        if (!float.TryParse(input, out float distance))
        {
            errorMsg.text = "Entered value is not a valid number";
            return false;
        }

        if (distance > maxDistance)
        {
            errorMsg.text = $"The entered distance must be ≤ {maxDistance}";
            return false;
        }

        if (distance < minDistance)
        {
            errorMsg.text = $"The entered distance must be ≥ {minDistance}";
            return false;
        }

        number = distance;
        return true;
    }
    private IEnumerator MoveLens(Vector3 targetPos, bool distanceCorrect)
    {
        while (lensTransform.localPosition != targetPos)
        {
            lensTransform.localPosition = Vector3.MoveTowards(
                lensTransform.localPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (distanceCorrect)
        {
            animateLampRay.DistanceCorrect();
        }

        animateLampRay.EnableLamp();
    }
}
