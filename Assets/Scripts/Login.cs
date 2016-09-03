using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Login : MonoBehaviour {
    [SerializeField]
    private InputField accountNameInputField;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private Toggle rememberAccountNameToggle;

    private Prompt prompt;

    public EventSystem system;
    private Selectable next = null;


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
        //Initialize the prompt
        prompt = Prompt.instance;
        prompt.Initialize(Groups.Login);

        //Set the color
        UIGroups.SetColor(Groups.Prompt, 2, true);
        UIGroups.SetColor(Groups.Login, 2, true);

        //Fade in the login
        StartCoroutine(UIGroups.FadeIn(Groups.Login, 0.5f));

        system = EventSystem.current;

        //Give the account inputfield the focus
        GameObject accountInputField = GameObject.Find("Account InputField");
        system.SetSelectedGameObject(accountInputField, new BaseEventData(system));
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            else
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }
}
