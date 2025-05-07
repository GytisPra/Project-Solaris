using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class UnlockEntry
{
    public string planetName;
    public int days;
}

[System.Serializable]
public class UnlockResponse
{
    public UnlockEntry[] unlocked;
}

public class UnlockManager : MonoBehaviour
{
    public PlanetSelectionUI planetSelectionUI;

    private Coroutine pollingCoroutine;

    private void OnEnable()
    {
        pollingCoroutine = StartCoroutine(PollUnlocksLoop());
        GameStateManager.Instance.SetState(GameState.Menu);
    }

    private void OnDisable()
    {
        if (pollingCoroutine != null)
            StopCoroutine(pollingCoroutine);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private IEnumerator PollUnlocksLoop()
    {
        while (true)
        {
            yield return FetchAndUnlockPlanets();
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator FetchAndUnlockPlanets()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://project-solaris-shop-production.up.railway.app/unlocks");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Unlock data: " + request.downloadHandler.text);

            var json = request.downloadHandler.text;
            UnlockResponse data = JsonUtility.FromJson<UnlockResponse>(json);

            foreach (UnlockEntry entry in data.unlocked)
            {
                Debug.Log($"Planet: {entry.planetName}, Days: {entry.days}");
                planetSelectionUI.UnlockPlanet(entry.planetName, entry.days);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch unlocks: " + request.error);
        }
    }
}
