using System.Collections;
using UnityEngine;

public class PlayerScaler : MonoBehaviour
{
    public float sizeIncreaseSpeed = 0.5f;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject levelUIManager;

    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.useGravity = false;
            playerRb.isKinematic = true;
        }

        player.localScale = Vector3.zero;


        StartCoroutine(ScalePlayer(Vector3.one));
    }

    private IEnumerator ScalePlayer(Vector3 targetScale)
    {
        GameStateManager.Instance.SetState(GameState.Cutscene);
        levelUIManager.SetActive(false);

        // Wait for the white screen fade to complete
        while (LoadingScreenManager.Instance.IsFading)
            yield return null;

        // Smoothly scale player to target scale
        while (!Mathf.Approximately(Vector3.Distance(player.localScale, targetScale), 0f))
        {
            player.localScale = Vector3.MoveTowards(
                player.localScale,
                targetScale,
                sizeIncreaseSpeed * Time.deltaTime
            );

            yield return null;
        }

        player.localScale = targetScale;

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;
        }

        // Reactivate UI and set gameplay state
        levelUIManager.SetActive(true);
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
}
