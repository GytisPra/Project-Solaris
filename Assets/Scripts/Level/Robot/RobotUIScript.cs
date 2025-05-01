using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RobotUIScript : MonoBehaviour
{
    public event Action OnRobotInteracted;
    public Canvas conversationCanvas;
    public LookController lookController;

    [Header("Cameras for transitions")]
    public Camera thirdPersonCamera;
    public Camera conversationCamera;

    public void InteractWithRobot()
    {
        OnRobotInteracted?.Invoke();

        // Changing the gameState to conversation
        // will begin the rotation of the player character to face the robot
        GameStateManager.Instance.SetState(GameState.Conversation);
        conversationCamera.gameObject.SetActive(true);
        StartCoroutine(TransitionCamera());
    }

    private IEnumerator TransitionCamera()
    {
        // Wait for the rotation of the character to finish
        yield return new WaitWhile(() => !lookController.finishedRotation);

        // Transition to conversation camera and enable the canvas
        yield return StartCoroutine(CameraTransition.Instance.SmoothCameraTransition(
            thirdPersonCamera,     // fromCamera
            conversationCamera,    // toCamera
            1,                     // duration
            GameState.Conversation // GameState after transition
        ));
        conversationCanvas.gameObject.SetActive(true);
    }

    public void ExitConversation()
    {
        SolarPad.Instance.UnlockSubject("TEST");

        conversationCanvas.gameObject.SetActive(false);

        StartCoroutine(CameraTransition.Instance.TransitionBack(1));
    }
}
