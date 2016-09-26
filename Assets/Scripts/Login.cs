using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Login : Scene {
    [SerializeField]
    private InputField accountNameInputField;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private Toggle rememberAccountNameToggle;

    [SerializeField]
    private UISelectable loginButton;

    [SerializeField]
    private Text loginButtonText;

    [SerializeField]
    private Text accountLabel;

    [SerializeField]
    private Text passwordLabel;

    [SerializeField]
    private Text rememberAccountNameLabel;

    [SerializeField]
    private Text accountMenuButtonLabel;

    [SerializeField]
    private Text optionsMenuButtonLabel;

    [SerializeField]
    private Text quitMenuButtonLabel;

    private bool accountInputFieldHasText = false;
    private bool passwordInputFieldHasText = false;
    private IEnumerator enterListener;


    void Awake()
    {
        //Is the remember account toggle on or off?
        bool rememberAccount = System.Convert.ToBoolean(PlayerPrefs.GetInt("RememberAccount"));
        rememberAccountNameToggle.isOn = rememberAccount;
        
        //If remember account toggle is on, display the account name
        if(rememberAccount) accountNameInputField.text = PlayerPrefs.GetString("AccountName");
        
    }

    void Start()
    {
        SetScene(Groups.Login, 2, GameObject.Find("Account InputField"));

        //Assign the text
        accountLabel.text = Localization.key.accountLabel;
        loginButtonText.text = Localization.key.loginButton;
        passwordLabel.text = Localization.key.passwordLabel;
        rememberAccountNameLabel.text = Localization.key.rememberAccountNameLabel;
        accountMenuButtonLabel.text = Localization.key.accountMenuButton;
        optionsMenuButtonLabel.text = Localization.key.optionsMenuButton;
        quitMenuButtonLabel.text = Localization.key.quitMenuButton;
    }

    public void OnLoginClick()
    {
        //Is the account name and password valid inputs?
        bool isAccountNameValid = Regex.IsMatch(accountNameInputField.text, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");
        bool isPasswordValid = Regex.IsMatch(password.text, @"^(?=[-_a-zA-Z0-9]*?[A-Z])(?=[-_a-zA-Z0-9]*?[a-z])(?=[-_a-zA-Z0-9]*?[0-9])\S{8,16}$");


        //If both account name and password are invalid, prompt an error
        if(!isAccountNameValid || !isPasswordValid)
        {
            PromptInfo promptInfo = new PromptInfo
            {
                promptTitle = "Login",
                promptDescription = "The account name or password you entered is invalid. Please enter a valid account name and password.",
                button1Text = "Ok",
                button1Action = delegate { ClearInputFields(); },
                xButtonAction = delegate { ClearInputFields(); }
            };

            prompt.cancelDelegate = delegate { ClearInputFields(); };
            prompt.Show(promptInfo);
            return;
        }
        else
        {
            StartCoroutine(Authenticate(accountNameInputField.text, password.text));
        }
    }


    IEnumerator Authenticate(string accountName, string password)
    {
        //Submit the account name and password to be authenticated
        WWWForm form = new WWWForm();
        form.AddField("Account", Encryption.Encrypt(accountName + "|" + password));

        WWW www = new WWW(GameManager.phpURL + "Authenticate.php", form);

        yield return www;

        //Decrypt the string
        string decryptString = Encryption.Decrypt(www.text);
        
        
        //Get the data
        string[] data = decryptString.Split("|"[0]);


        //Validate to see if this is a number
        int numCharacters;
        bool isNumber = int.TryParse(data[0], out numCharacters);

        if (!isNumber)
        {
            InvalidLogin();
            yield break;
        }


        //Validate the player ID
        if (!Regex.IsMatch(data[1], "^[a-zA-Z0-9]{32}$"))
        {
            InvalidLogin();
            yield break;
        }


        //Assign the player ID
        GameManager.playerID = data[1];

        if(rememberAccountNameToggle.isOn)
        {
            PlayerPrefs.SetString("AccountName", accountName);
        }


        //If the player has at least one character, load the characters scene
        //else, load the character creation screen
        if (numCharacters > 0)
        {
            //StartCoroutine(sceneLoader.LoadScene(3));
            SceneManager.LoadScene(2);
        }
        else
        {
            //StartCoroutine(sceneLoader.LoadScene(2));

        }
        
    }

    public void OnRememberAccountClick()
    {
        //Store the OnRememberAccountClick toggle value in the registry
        int rememberAccountNamevalue = System.Convert.ToInt32(rememberAccountNameToggle.isOn);
        PlayerPrefs.SetInt("RememberAccount", rememberAccountNamevalue);

        //If the toggle value is off, clear the account name stored in the registry
        if (rememberAccountNamevalue == 0)
        {
            PlayerPrefs.SetString("AccountName", string.Empty);
        }
    }

    void InvalidLogin()
    {
        PromptInfo promptInfo = new PromptInfo
        {
            promptTitle = "Login",
            promptDescription = "The information you have entered is invalid. Check the spelling of the account name and password.",
            button1Text = "Ok",
            button1Action = delegate { }
        };

        prompt.Show(promptInfo);
    }

    public void OnAccountInputFieldChange()
    {
        InputFieldChange(accountNameInputField, ref accountInputFieldHasText, password);
    }

    public void OnPasswordInputFieldChange()
    {
        InputFieldChange(password, ref passwordInputFieldHasText, accountNameInputField);
    }

    private void InputFieldChange(InputField inputField1, ref bool hasText, InputField inputField2)
    {
        //If input field is NOT empty
        if (!string.IsNullOrEmpty(inputField1.text))
        {
            //As long as the change has occured for the first time from a null string state
            if (!hasText)
            {
                //If the other input field is also NOT empty
                if (!string.IsNullOrEmpty(inputField2.text))
                {
                    //Declare that the input field has text entered
                    hasText = true;

                    //Enable the login button
                    for (int i = 0; i < loginButton.children.Length; i++)
                    {
                        loginButton.OnOut((UIInteractiveGraphic)loginButton.children[i]);
                    }
                    loginButton.selectableComponent.interactable = true;

                    //Run coroutine
                    if (enterListener != null) StopCoroutine(enterListener);
                    enterListener = EnterListener();
                    StartCoroutine(enterListener);
                }
            }
        }

        //If the input field is empty
        else
        {
            //Declare that the input field no longer has text entered
            hasText = false;

            //Disable the login button
            for (int i = 0; i < loginButton.children.Length; i++)
            {
                loginButton.OnDisabled((UIInteractiveGraphic)loginButton.children[i]);
            }
            loginButton.selectableComponent.interactable = false;

            //Stop coroutine
            if (enterListener != null) StopCoroutine(enterListener);
        }
    }

    IEnumerator EnterListener()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if (enterListener != null) StopCoroutine(enterListener);
                if(!prompt.isVisible)OnLoginClick();
            }
            yield return null;
        }
    }

    void ClearInputFields()
    {
        accountNameInputField.text = string.Empty;
        password.text = string.Empty;

        currentSelectedGameObject = accountNameInputField.gameObject;
        SetSelectedGameObject();
    }

}
