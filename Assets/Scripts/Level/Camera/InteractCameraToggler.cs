using UnityEngine;

public class InteractCameraToggler : MonoBehaviour
{
    private Camera thisCamera;

    private void Start()
    {
        thisCamera = transform.GetComponent<Camera>();

        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState newState)
    {
        if (thisCamera == null) return;

        if (newState != GameState.Gameplay)
        {
            // Hide UI and UIExtra layers from this camera
            thisCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            thisCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UIExtra"));
        }
        else
        {
            // Show UI and UIExtra layers
            thisCamera.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
            thisCamera.cullingMask |= (1 << LayerMask.NameToLayer("UIExtra"));
        }
    }
}

