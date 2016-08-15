using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class CharacterCreation : MonoBehaviour
{
    [SerializeField]
    private InputField characterName;

    

    private Race[] race = new Race[10];
    private int currentRaceIndex;
    private IEnumerator rotation;

    // Use this for initialization
    public IEnumerator GetRaceData()
    {
        GameObject[] raceDisplayModel = GameManager.assets["RaceDisplayModels"].GetComponent<RaceDisplayModels>().raceDisplayModel;


        //Download information about all the races
        WWW www = new WWW(GameManager.phpURL + "Races.php");
        yield return www;

        //Decrypt the string
        string decryptString = Encryption.Decrypt(www.text);

        //Put the race data into an array
        string[] raceData = decryptString.Split("|"[0]);

        //Assign the data to each race
        int raceIndex = 0;
        for(int i = 0; i < raceData.Length - 1; i += 4)
        {
            race[raceIndex] = new Race();
            race[raceIndex].model = Instantiate(raceDisplayModel[int.Parse(raceData[i])]);
            race[raceIndex].model.transform.rotation = Quaternion.Euler(new Vector3(0, 20, 0));
            race[raceIndex].name = raceData[i + 1];
            race[raceIndex].description = raceData[i + 2];
            race[raceIndex].teamColor = (TeamColor)System.Enum.Parse(typeof(TeamColor), raceData[i + 3]);
            raceIndex++;
        }

        //Pick a random race to display
        currentRaceIndex = Random.Range(0, race.Length);
        race[currentRaceIndex].model.SetActive(true);

    }

    public void OnRaceButtonClick(int index)
    {   
        if(index != currentRaceIndex)
        {
            //Reset the current model's rotatation and disable
            race[currentRaceIndex].model.transform.rotation = Quaternion.Euler(new Vector3(0, 20, 0));
            race[currentRaceIndex].model.SetActive(false);

            //Enable the current model that was clicked
            currentRaceIndex = index;
            race[currentRaceIndex].model.SetActive(true);
        }
        
    }


    public void OnRandomizeButtonClick()
    {
        StartCoroutine(GetRandomName());
    }

    IEnumerator GetRandomName()
    {
        //Download a random name to display
        WWW www = new WWW(GameManager.phpURL + "Random_Name.php");
        yield return www;

        //Decrypt the string
        string decryptString = Encryption.Decrypt(www.text);

        //Display the random name
        characterName.text = decryptString;
    }

    public void OnCreateButtonClick()
    {
        //Make sure the length of the name is at least 2 characters long
        int length = characterName.text.Trim().Length;
        if (length < 2)
        {
            Debug.Log("Names must be at least two characters");
            return;
        }


        //Make sure the name only contains letters
        bool isCharacterNameValid = Regex.IsMatch(characterName.text.Trim(), "^[a-zA-Z]+$");
        if (!isCharacterNameValid)
        {
            Debug.Log("Names can only contain letters");
            return;
        }

        //Submit the character name
        StartCoroutine(CreateCharacter(characterName.text.Trim()));
    }


    IEnumerator CreateCharacter(string characterName)
    {
        //The data we will be passing in to create the new character
        WWWForm form = new WWWForm();
        form.AddField("CharacterData", Encryption.Encrypt(GameManager.playerID + "|" + characterName + "|" + currentRaceIndex));
        

        //Download the results from the database
        WWW www = new WWW(GameManager.phpURL + "CreateCharacter.php", form);
        yield return www;


        
        //Decrypt the string
        string decryptString = Encryption.Decrypt(www.text);



        //Validate to see if this is a number
        int number;
        bool isNumber = int.TryParse(decryptString, out number);

        
        //If we get a number then load the characters
        if (isNumber)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            Debug.Log("That name is unavailable");

        }

    }


    public void OnRotateButtonDown(int buttonIndex)
    {
        float direction = 0;

        //Left button clicked: Rotate clockwise
        if (buttonIndex == 0) direction = 1;

        //Right button clicked: rotate counterclockwise
        if (buttonIndex == 1) direction = -1;

        //Rotate the current model
        rotation = Rotation(direction);
        StartCoroutine(rotation);
    }

    public void OnRotateButtonUp()
    {
        StopCoroutine(rotation);
    }

    IEnumerator Rotation(float direction)
    {
        while (true)
        {
            RotateModel(direction);
            yield return null;
        }

    }


    void RotateModel(float direction)
    {
        race[currentRaceIndex].model.transform.Rotate(new Vector3(0, 2 * direction, 0));
    }

    void Update()
    {
        float speed = 5;

        if (Input.GetMouseButton(0))
        {
            RotateModel(Input.GetAxis("Mouse X") * -speed);
        }
    }
}
