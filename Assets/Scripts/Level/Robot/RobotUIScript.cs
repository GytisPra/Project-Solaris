using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RobotUIScript : MonoBehaviour
{
    public event Action OnRobotInteracted;
    public Canvas conversationCanvas;
    public Canvas interactCanvas;
    public CharLookController lookController;
    public GameObject lightBulb;

    [Header("Cameras for transitions")]
    public Camera thirdPersonCamera;
    public Camera conversationCamera;

    private ConversationCamera convCamScript;

    void Start()
    {
        convCamScript = conversationCamera.gameObject.GetComponent<ConversationCamera>();
    }

    public void InteractWithRobot()
    {
        lightBulb.SetActive(false); // Hide the light bulb so that it doesn't show up anymore

        OnRobotInteracted?.Invoke();

        interactCanvas.gameObject.SetActive(false);

        // Changing the gameState to conversation
        // will begin the rotation of the player character to face the robot
        GameStateManager.Instance.SetState(GameState.Conversation);

        StartCoroutine(TransitionCamera()); // Start camera transitioning
    }

    private IEnumerator TransitionCamera()
    {
        conversationCamera.gameObject.SetActive(true); // Eanble the camera

        // Wait for the rotation of the character to finish
        yield return new WaitWhile(() => !lookController.finishedRotation);

        convCamScript.PrepareForConversation();
        // wait one frame to ensure transform updates are processed
        yield return new WaitForEndOfFrame();

        // Transition to conversation camera and enable the canvas
        yield return StartCoroutine(CameraTransition.Instance.SmoothCameraTransition(
            thirdPersonCamera,     // fromCamera
            conversationCamera,    // toCamera
            1,                     // duration
            GameState.Conversation // GameState after transition
        ));
        conversationCanvas.gameObject.SetActive(true);
    }
}
