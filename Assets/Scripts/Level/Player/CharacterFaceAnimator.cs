using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CharacterFaceAnimator : MonoBehaviour
{
    [Header("Materials")]
    public Material eyesMaterial;
    public Material mouthMaterial;

    [Header("Eyes Textures")]
    public Texture2D openMouthTexture;
    public Texture2D closedMouthTexture;

    [Header("Mouth Textures")]
    public Texture2D openEyesTexture;
    public Texture2D closedEyesTexture;

    [Header("Settings")]
    public float minBlinkInterval = 2f;
    public float maxBlinkInterval = 5f;
    public float blinkDuration = 0.1f;

    private float blinkTimer;
    private bool isBlinking = false;

    private float mouthTimer;
    void Start()
    {
        ResetBlinkTimer();
        ResetMouthTimer();
    }

    void Update()
    {
        HandleBlinking();
        AnimateMouth();
    }

    void HandleBlinking()
    {
        blinkTimer -= Time.deltaTime;

        if (!isBlinking && blinkTimer <= 0f)
        {
            StartCoroutine(Blink());
            ResetBlinkTimer();
        }
    }

    System.Collections.IEnumerator Blink()
    {
        isBlinking = true;
        eyesMaterial.SetTexture("_Texture2D", closedEyesTexture);

        yield return new WaitForSeconds(blinkDuration);

        eyesMaterial.SetTexture("_Texture2D", openEyesTexture);
        isBlinking = false;
    }

    void AnimateMouth()
    {
        mouthTimer -= Time.deltaTime;
        if (mouthTimer <= 0f)
        {
            bool isOpen = Random.value > 0.5f;
            mouthMaterial.SetTexture("_Texture2D", isOpen ? openMouthTexture : closedMouthTexture);
            ResetMouthTimer();
        }
    }

    void ResetBlinkTimer()
    {
        blinkTimer = Random.Range(minBlinkInterval, maxBlinkInterval);
    }

    void ResetMouthTimer()
    {
        mouthTimer = Random.Range(1.0f, 3.0f);
    }
}
