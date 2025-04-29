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

    [Header("Cameras for transition")]
    [SerializeField] private Camera fromCamera;
    [SerializeField] private Camera toCamera;

    public void Close()
    {
        levelUIManager.SetInteractionPopupActive(false);
        levelUIManager.SetLevelUICanvasActive(true);
        levelUIManager.SetDepthOfFieldEffectActive(false);
        levelUIManager.SetNearLensCanvasActive(true);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void Confirm()
    {
        bool distanceCorrect = false;
        string inputedText = inputField.text.Replace('.', ',');

        if (ValidateInput(inputedText, out float distance))
        {
            levelUIManager.SetInteractionPopupActive(false);
            levelUIManager.SetLevelUICanvasActive(true);
            levelUIManager.SetDepthOfFieldEffectActive(false);
            levelUIManager.SetNearLensCanvasActive(true);


            StartCoroutine(WaitForCameraTransition(distance, distanceCorrect, targetDistance));
        }
    }

    private IEnumerator WaitForCameraTransition(float distance, bool distanceCorrect, float targetDistance)
    {
        yield return StartCoroutine(CameraTransition.Instance.SmoothCameraTransition(fromCamera, toCamera));

        if (distance == targetDistance)
        {
            distanceCorrect = true;
        }

        Vector3 targetPos = new(lensTransform.localPosition.x, lensTransform.localPosition.y, -distance);

        yield return StartCoroutine(MoveLens(targetPos, distanceCorrect));

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
    public void OnValueChanged()
    {
        StartCoroutine(animateLampRay.DisableLamp());
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

        StartCoroutine(animateLampRay.EnableLamp());
    }
}
