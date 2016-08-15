using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Prompt : MonoBehaviour {
    private UIPrompt prompt;
    private Groups fadeOutGroup;
    private float PromptFadeTime = 0.25f;

    public static Prompt instance
    {
        get
        {
            return (Prompt)FindObjectOfType(typeof(Prompt));
        }
    }
    

    // Use this for initialization
    void Awake ()
    {
        DontDestroyOnLoad(gameObject);
        prompt = transform.GetChild(0).GetComponent<UIPrompt>();
    }

    public void Initialize(Groups fadeOutGroup)
    {
        GameObject canvas = GameObject.Find("Canvas");
        transform.SetParent(canvas.transform, false);
        gameObject.SetActive(false);
        this.fadeOutGroup = fadeOutGroup;
    }

    public void Show(PromptInfo promptInfo)
    {
        //Position the prompt in the center of the screen
        RectTransform rectTransform = (RectTransform)prompt.transform;
        rectTransform.anchoredPosition = Vector3.zero;

        //Show the prompt
        gameObject.SetActive(true);
        prompt.panel.SetActive(false);

        //Set the title and description
        prompt.promptTitleText.text = promptInfo.promptTitle;
        prompt.promptDescriptionText.text = promptInfo.promptDescription;

        //XButton
        prompt.xButton.onClick.RemoveAllListeners();
        prompt.xButton.onClick.AddListener(Hide);

        //Button1
        prompt.button1.buttonText.text = promptInfo.button1Text;
        prompt.button1.buttonComponent.onClick.RemoveAllListeners();
        prompt.button1.buttonComponent.onClick.AddListener(promptInfo.button1Action);
        prompt.button1.buttonComponent.onClick.AddListener(Hide);

        if (promptInfo.button2Text == null)
        {
            prompt.button2.buttonComponent.gameObject.SetActive(false);
        }
        else
        {
            //Button2
            prompt.button2.buttonText.text = promptInfo.button2Text;
            prompt.button2.buttonComponent.onClick.RemoveAllListeners();
            prompt.button2.buttonComponent.onClick.AddListener(promptInfo.button2Action);
            prompt.xButton.onClick.AddListener(promptInfo.button2Action);
            prompt.button2.buttonComponent.onClick.AddListener(Hide);
        }


        StartCoroutine(UIGroups.FadeIn(Groups.Prompt, PromptFadeTime));
        StartCoroutine(UIGroups.FadeOut(fadeOutGroup, 0.05f, PromptFadeTime));
    }


    private void Hide()
    {
        prompt.panel.SetActive(true);
        StartCoroutine(HidePrompt());
    }

    private IEnumerator HidePrompt()
    {
        StartCoroutine(UIGroups.FadeIn(fadeOutGroup, PromptFadeTime));
        yield return StartCoroutine(UIGroups.FadeOut(Groups.Prompt, 0.05f, PromptFadeTime));
        gameObject.SetActive(false);
    }

}


[System.Serializable]
public struct PromptButton
{
    public Button buttonComponent;
    public Text buttonText;
}

public struct PromptInfo
{
    public string promptTitle;
    public string promptDescription;

    //Button1
    public string button1Text;
    public UnityAction button1Action;

    //Button2
    public string button2Text;
    public UnityAction button2Action;
}
