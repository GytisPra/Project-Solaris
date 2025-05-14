using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VectorGraphics;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public Image whiteScreen;
    public SVGImage gameLogo;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade In
        yield return StartCoroutine(Fade(0f, 1f));

        // Load Scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade Out
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color whiteScreenColor = whiteScreen.color;
        Color gameLogoColor = gameLogo.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            whiteScreenColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            gameLogoColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            whiteScreen.color = whiteScreenColor;
            gameLogo.color = gameLogoColor;
            yield return null;
        }

        whiteScreenColor.a = endAlpha;
        gameLogoColor.a = endAlpha;
        whiteScreen.color = whiteScreenColor;
        gameLogo.color = gameLogoColor;
    }
}
