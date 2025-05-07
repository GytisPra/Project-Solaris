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
        public string content;
    }

    public InteractTrigger interactTrigger;

    private bool isFirstConversation = true;
    private bool isTyping = false;

    [Header("UI Components")]
    public TMP_Text text;
    public Button continueButton;

    [Header("Dialog Settings")]
    public List<Page> pages = new();
    public float waitAfterLetter = 0.05f;

    private int numberOfPages = 0;
    private int currentPageNumber = 0;

    private readonly Color greenColor = new(0.5f, 1f, 0.4f, 0.5f);
    private readonly Color redColor = new(1f, 0.4f, 0.4f, 0.5f);

    void OnEnable()
    {
        if (!isFirstConversation)
        {
            text.alignment = TextAlignmentOptions.Center;

            StopAllCoroutines();
            StartCoroutine(TypeText("Check the SolarPad!"));
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
            StartCoroutine(TypeText(page.content));
        }
    }

    private Page GetPage(int pageNumber)
    {
        return pages.Find(p => p.pageNumber == pageNumber);
    }

    private IEnumerator TypeText(string content)
    {
        isTyping = true;
        text.text = "";

        if (currentPageNumber + 1 >= numberOfPages && isFirstConversation)
        {
            SetupButton("Exit", redColor, ExitConversation);
            isFirstConversation = false;
        }

        foreach (char letter in content)
        {
            text.text += letter;
            yield return new WaitForSeconds(waitAfterLetter);

            // Allow skipping if interrupted
            if (!isTyping)
            {
                if (currentPageNumber + 1 >= numberOfPages && isFirstConversation)
                {
                    SetupButton("Exit", redColor, ExitConversation);
                    isFirstConversation = false;
                }

                text.text = content;

                // Force layout update so UI resizes properly
                RectTransform parentRect = text.transform.parent as RectTransform;
                if (parentRect != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                }

                yield break;
            }
        }

        isTyping = false;
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

        StopAllCoroutines();
        StartCoroutine(TypeText(page.content));
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

        SolarPad.Instance.UnlockSubject("TEST");
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
