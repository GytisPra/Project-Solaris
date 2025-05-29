using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogUIScript : MonoBehaviour
{
    [System.Serializable]
    public class Page
    {
        public int pageNumber;
        [TextArea(3, 10)]
        public string content;
        public Sprite formulaSprite;
    }

    public InteractTrigger interactTrigger;
    public string repeatText = "If you forget anything, you can find a list of physics theories in your SolarPad. Just click the button in the top-right corner of your screen to open it.";

    private bool isFirstConversation = true;
    private bool isTyping = false;

    [Header("UI Components")]
    public TMP_Text text;
    public Button continueButton;
    public Image formulaUIImage;

    [Header("Dialog Settings")]
    public List<Page> pages = new();
    public float waitAfterLetter = 0.05f;
    public string unlockSubject;

    private int numberOfPages = 0;
    private int currentPageNumber = 0;

    private readonly Color greenColor = new(0.5f, 1f, 0.4f, 0.5f);
    private readonly Color redColor = new(1f, 0.4f, 0.4f, 0.5f);

    void OnEnable()
    {
        if (!isFirstConversation)
        {
            StopAllCoroutines();
            StartCoroutine(TypeTextRich(repeatText, null));
            SetupButton("Exit", redColor, ExitConversation);
            return;
        }

        text.alignment = TextAlignmentOptions.TopLeft;
        SetupButton("Next", greenColor, NextPage);

        numberOfPages = pages.Count;
        currentPageNumber = 0;

        if (numberOfPages <= 0)
        {
            Debug.LogWarning("No pages assigned for the conversation");
            return;
        }

        Page page = GetPage(currentPageNumber);
        if (page != null)
        {
            StopAllCoroutines();
            StartCoroutine(TypeTextRich(page.content, page.formulaSprite));
        }
    }

    private Page GetPage(int pageNumber)
    {
        return pages.Find(p => p.pageNumber == pageNumber);
    }

    private IEnumerator TypeTextRich(string content, Sprite formulaSprite)
    {
        isTyping = true;
        text.text = "";

        if (currentPageNumber + 1 >= numberOfPages && isFirstConversation)
        {
            SetupButton("Exit", redColor, ExitConversation);
            isFirstConversation = false;
        }

        string fullText = "";
        List<string> openTags = new();

        for (int i = 0; i < content.Length; i++)
        {
            char letter = content[i];

            if (letter == '\\' && i + 1 < content.Length)
            {
                char nextLetter = content[i + 1];
                if (nextLetter == 'n')
                {
                    fullText += '\n';
                    i++;
                }
                else
                {
                    fullText += '\\';
                }
            }
            else if (letter == '<')
            {
                int closeIndex = content.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    string tag = content.Substring(i, closeIndex - i + 1);
                    fullText += tag;

                    if (!tag.StartsWith("</"))
                    {
                        openTags.Add(tag);
                    }
                    else if (openTags.Count > 0)
                    {
                        openTags.RemoveAt(openTags.Count - 1);
                    }

                    i = closeIndex;
                    continue;
                }
            }
            else
            {
                fullText += letter;
            }

            text.text = BuildRichText(fullText, openTags);
            yield return new WaitForSeconds(waitAfterLetter);

            if (!isTyping)
            {
                text.text = content;
                if (formulaSprite != null)
                {
                    formulaUIImage.gameObject.SetActive(true);
                    formulaUIImage.sprite = formulaSprite;
                }
                yield break;
            }
        }

        if (formulaSprite != null)
        {
            formulaUIImage.gameObject.SetActive(true);
            formulaUIImage.sprite = formulaSprite;
        }

        isTyping = false;
    }

    private string BuildRichText(string visibleText, List<string> openTags)
    {
        string result = visibleText;
        for (int i = openTags.Count - 1; i >= 0; i--)
        {
            string tag = openTags[i];
            if (tag.StartsWith("<") && !tag.StartsWith("</"))
            {
                string tagName = tag[1..^1].Split(' ')[0];
                result += $"</{tagName}>";
            }
        }
        return result;
    }

    public void NextPage()
    {
        if (isTyping)
        {
            isTyping = false;
            return;
        }

        currentPageNumber++;
        Page page = GetPage(currentPageNumber);

        if (page == null)
        {
            return;
        }

        formulaUIImage.gameObject.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(TypeTextRich(page.content, page.formulaSprite));
    }

    public void SkipText()
    {
        if (isTyping)
        {
            isTyping = false;
            return;
        }
    }

    public void ExitConversation()
    {
        if (isTyping)
        {
            isTyping = false;
            return;
        }
        if (string.IsNullOrWhiteSpace(unlockSubject))
        {
            Debug.LogWarning($"Unlock subject: {unlockSubject} is null or whitespace!");
        }
        else
        {
            SolarPad.Instance.UnlockSubject(unlockSubject);
        }
            
        StartCoroutine(CameraTransition.Instance.TransitionBack(0.5f));
        interactTrigger.ShowInteract();
        gameObject.SetActive(false);
    }

    private void SetupButton(string buttonText, Color color, UnityAction action)
    {
        TMP_Text buttonTextComponent = continueButton.GetComponentInChildren<TMP_Text>();
        if (buttonTextComponent != null)
            buttonTextComponent.text = buttonText;

        ColorBlock cb = continueButton.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.selectedColor = color;
        cb.pressedColor = new Color(color.r, color.g, color.b, 1f);
        continueButton.colors = cb;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(action);
    }
}