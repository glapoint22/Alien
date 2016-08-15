using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DeleteCharacterPrompt : MonoBehaviour
{
    [SerializeField]
    private Text message;

    [SerializeField]
    InputField inputField;

    [SerializeField]
    private Button deleteButton;



    void OnEnable()
    {
        inputField.text = string.Empty;
        message.text = "Are you sure you want to delete this character?\n\nType \"" + Characters.selectedCharacter.name + "\" into this field to confirm.";
    }


    public void ClosePrompt()
    {
        transform.parent.gameObject.SetActive(false);
        deleteButton.interactable = false;
    }


    public void OnValueChange()
    {
        if(inputField.text.ToLower() == Characters.selectedCharacter.name.ToLower())
        {
            deleteButton.interactable = true;
        }
        else
        {
            deleteButton.interactable = false;
        }
    }


    public void OnDeleteButtonClick()
    {
        StartCoroutine(DeleteCharacter());
    }


    IEnumerator DeleteCharacter()
    {
        //The data we will be passing in to create the new character
        WWWForm form = new WWWForm();
        form.AddField("CharacterData", Encryption.Encrypt(GameManager.playerID + "|" + Characters.selectedCharacter.id)); 


        //Download the results from the database
        WWW www = new WWW(GameManager.phpURL + "DeleteCharacter.php", form);
        yield return www;



        //Decrypt the string
        string decryptString = Encryption.Decrypt(www.text);

        //Invalid field was entered
        if (decryptString == "INVALID FIELD")
        {
            Debug.Log("Invalid field");
        }

        //Decryption error
        else if (decryptString == "DECRYPTION ERROR")
        {
            Debug.Log("Decryption error");
        }


        DestroyGameObjects();

    }


    void DestroyGameObjects()
    {
        //Destroy the character select button
        Destroy(Characters.selectedCharacter.characterSelectButton.gameObject);

        //Destroy the armor slots
        for (int i = 0; i < Characters.selectedCharacter.equipSlot.Length; i++)
        {
            Destroy(Characters.selectedCharacter.equipSlot[i].gameObject);
        }


        //Destroy the inventory slots
        Dictionary<string, InventorySlot>.ValueCollection inventorySlots = Characters.selectedCharacter.inventorySlot.Values;
        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            Destroy(inventorySlot.gameObject);
        }

        //Destroy the character model
        Destroy(Characters.selectedCharacter.skinnedMeshRenderer.gameObject);


        //Remove the selected character from the list
        Characters.character.Remove(Characters.selectedCharacter);



        //Set the selected character as the first character in the character list
        Characters.selectedCharacter = Characters.character[0];

        //Enable the skinnedMeshRenderer
        Characters.selectedCharacter.skinnedMeshRenderer.enabled = true;

        //Enable the armor slots for the selected character
        for (int i = 0; i < Characters.selectedCharacter.armor.Length; i++)
        {
            Characters.selectedCharacter.equipSlot[i].gameObject.SetActive(true);
        }


        //Enable the current character's inventory slots
        Dictionary<string, InventorySlot>.ValueCollection selectedCharacterInventorySlots = Characters.selectedCharacter.inventorySlot.Values;
        foreach (InventorySlot inventorySlot in selectedCharacterInventorySlots)
        {
            inventorySlot.gameObject.SetActive(true);
        }


        //Close the prompt
        ClosePrompt();
    }
}
