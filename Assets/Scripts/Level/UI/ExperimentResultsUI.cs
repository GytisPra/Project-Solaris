using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
public class ExperimentResultsUI : MonoBehaviour
{
    [SerializeField] private SVGImage successImage;
    [SerializeField] private SVGImage failedImage;

    public float delayBeforeFadingOut = 0.5f;
    public float fadeDuration;
    
    public IEnumerator DisplayResult(bool isSuccess)
    {
        SVGImage image = isSuccess ? successImage : failedImage;

        // Fade In
        yield return StartCoroutine(Fade(0f, 1f, image));

        // Wait a little
        yield return new WaitForSeconds(delayBeforeFadingOut);

        // Fade Out
        yield return StartCoroutine(Fade(1f, 0f, image));
    }

    IEnumerator Fade(float startAlpha, float endAlpha, SVGImage image)
    {
        float elapsed = 0f;
        Color imageColor = image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            imageColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            image.color = imageColor;
            yield return null;
        }

        imageColor.a = endAlpha;

        image.color = imageColor;
    }

}
