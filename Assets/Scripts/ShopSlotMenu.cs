using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopSlotMenu : SlotMenu
{
    private Shop shop;
    private Button resetArmorButton;
    private Inventory inventory;

    void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        resetArmorButton = GameObject.Find("Reset Armor Button").GetComponent<Button>();
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    public void OnMenuTryOnButtonClick()
    {
        //Equip the character with this armor piece
        Characters.selectedCharacter.EquipArmorPiece((Armor)shop.currentShopSlot.item);

        //Set variables
        gameObject.SetActive(false);
        resetArmorButton.interactable = true;

        //Set the current shop slot to null
        shop.currentShopSlot = null;
    }


    public void SetMenuButtons(Slot shopSlot)
    {
        //Reset all the buttons
        ResetMenuButtons();

        //If the item is stackable
        if (shopSlot.item.isStackable)
        {
            itemStack.stackCount = shopSlot.stackCount;
            itemStack.stackCap = shopSlot.item.stackCap;
            itemStack.stackCountInputField.text = shopSlot.stackCount.ToString();
            menuButton[1].SetActive(true);
        }
        else
        {
            itemStack.stackCount = 1;
            menuButton[0].SetActive(true);
        }


        //Weapon
        if (shopSlot.item is Weapon)
        {
            menuButton[2].SetActive(true);
        }


        //Armor
        else if (shopSlot.item is Armor)
        {
            menuButton[3].SetActive(true);
        }


    }

    public void OnMenuPurchaseButtonClick()
    {
        //Purchase button was clicked
        StartCoroutine(PurchaseItem());
        
    }


    IEnumerator PurchaseItem()
    {
        string data = string.Empty;

        //Get all the data into a string
        data += GameManager.playerID + "|";
        data += Characters.selectedCharacter.id + "|";
        data += shop.currentShopSlot.item.id + "|";
        data += itemStack.stackCount;

        //Send the data and download the results
        WWWForm form = new WWWForm();
        form.AddField("Data", Encryption.Encrypt(data));
        WWW www = new WWW(GameManager.phpURL + "Buy_Item.php", form);
        yield return www;

        string decryptString = Encryption.Decrypt(www.text);

        //Test for errors
        bool isError = false;
        switch (decryptString)
        {
            case "INVALID FIELD":
                Debug.Log("Invalid field");
                isError = true;
                break;
            case "INSUFFICIENT FUNDS":
                Debug.Log("Insufficient funds");
                isError = true;
                break;
            case "INVENTORY FULL":
                Debug.Log("Inventory full");
                isError = true;
                break;
            case "DECRYPTION ERROR":
                Debug.Log("Decryption error");
                isError = true;
                break;
        }

        if (isError) yield break;


        string[] stringArray = decryptString.Split("|"[0]);

        //Get the inventory data
        string[] inventoryData = new string[stringArray.Length - 1];
        System.Array.Copy(stringArray, inventoryData, stringArray.Length - 1);


        //Put this item in the inventory
        inventory.UpdateSlots(inventoryData, shop.currentShopSlot.item);


        //Adjust the player's currency
        Characters.selectedCharacter.currency = float.Parse(stringArray[stringArray.Length - 1]);


        //Set the current shop slot to null
        shop.currentShopSlot = null;

        gameObject.SetActive(false);

    }
}
