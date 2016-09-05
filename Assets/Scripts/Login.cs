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


    void Awake()
    {
        //Load the startup scene if we are trying to start the game with this scene
        if (UIGroups.uiGroup[0].elements == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        //Is the remember account toggle on or off?
        bool rememberAccount = System.Convert.ToBoolean(PlayerPrefs.GetInt("RememberAccount"));
        rememberAccountNameToggle.isOn = rememberAccount;
        
        //If remember account toggle is on, display the account name
        if(rememberAccount) accountNameInputField.text = PlayerPrefs.GetString("AccountName");
    }

    void Start()
    {
        SetScene(Groups.Login, 2, GameObject.Find("Account InputField"));
    }

    public void OnLoginClick()
    {
        //Check if account name is empty
        if(accountNameInputField.text == string.Empty)
        {
            PromptInfo promptInfo = new PromptInfo
            {
                promptTitle = "Login",
                promptDescription = "Please enter your account name.",
                button1Text = "Ok",
                button1Action = delegate {}
            };

            prompt.Show(promptInfo);
            return;
        }


        //Check if password is empty
        if (password.text == string.Empty)
        {
            PromptInfo promptInfo = new PromptInfo
            {
                promptTitle = "Login",
                promptDescription = "Please enter your password.",
                button1Text = "Ok",
                button1Action = delegate { }
            };


            prompt.Show(promptInfo);
            return;
        }

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
                button1Action = delegate { }
            };


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
}
