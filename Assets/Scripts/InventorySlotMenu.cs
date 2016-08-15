using UnityEngine;
using System.Collections;

public class InventorySlotMenu : SlotMenu
{
    [SerializeField]
    private Inventory inventory;


    public void SetMenuButtons(InventorySlot inventorySlot)
    {
        //Reset all the buttons
        ResetMenuButtons();

        //If the item has modifications
        if (inventorySlot.item.modifications.Count > 0)
        {
            menuButton[4].SetActive(true);
        }


        //If the item is stackable
        if (inventorySlot.item.isStackable)
        {
            itemStack.stackCount = inventorySlot.stackCount;
            itemStack.stackCap = inventorySlot.stackCount;
            itemStack.stackCountInputField.text = inventorySlot.stackCount.ToString();
            menuButton[1].SetActive(true);
        }
        else
        {
            itemStack.stackCount = 1;
            menuButton[0].SetActive(true);
        }


        //Weapon
        if (inventorySlot.item is Weapon)
        {
            menuButton[2].SetActive(true);
        }


        //Armor
        else if (inventorySlot.item is Armor)
        {
            menuButton[3].SetActive(true);
        }


    }


    public void OnMenuEquipButtonClick()
    {
        //Equip the armor piece
        StartCoroutine(Equip(inventory.currentInventorySlot));
        
    }


    public IEnumerator Equip(InventorySlot inventorySlot)
    {
        string data = string.Empty;

        //Get all the data into a string
        data += GameManager.playerID + "|";
        data += Characters.selectedCharacter.id + "|";
        data += inventorySlot.item.id + "|";
        data += inventorySlot.category + "|";
        data += inventorySlot.slotNum;

        //Send the data and download the results
        WWWForm form = new WWWForm();
        form.AddField("Data", Encryption.Encrypt(data));
        WWW www = new WWW(GameManager.phpURL + "Equip.php", form);
        yield return www;

        Armor armorPiece = (Armor)inventorySlot.item;

        //Decrypt the data
        string decryptString = Encryption.Decrypt(www.text);

        //Invalid field was entered
        if (decryptString == "INVALID FIELD")
        {
            Debug.Log("Invalid field");
            //yield break;
        }

        //Decryption error
        else if (decryptString == "DECRYPTION ERROR")
        {
            Debug.Log("Decryption error");
            //yield break;
        }


        //Armor slot is empty - equip the armor piece from the inventory into the armor slot
        else if (Characters.selectedCharacter.equipSlot[armorPiece.armorType].image.sprite == null)
        {
            EquipArmor((Armor)inventorySlot.item);
            Destroy(inventorySlot.equipMenuButton.gameObject);
            Destroy(Characters.selectedCharacter.inventorySlot[inventorySlot.category + "_" + inventorySlot.slotNum].gameObject);
            Characters.selectedCharacter.inventorySlot.Remove(inventorySlot.category + "_" + inventorySlot.slotNum);
        }
        else
        //Armor slot is NOT empty - Swap the equipped armor piece with the armor piece that is in the inventory
        {
            EquipArmor((Armor)inventorySlot.item);

            armorPiece = (Armor)Shop.shopSlot[decryptString].item;

            Characters.selectedCharacter.inventorySlot[inventorySlot.category + "_" + inventorySlot.slotNum].item = armorPiece;
            inventorySlot.equipMenuButton.image.sprite = armorPiece.image;
        }


        //Set the current inventory slot to null
        inventory.currentInventorySlot = null;

        gameObject.SetActive(false);
    }

    void EquipArmor(Armor armorPiece)
    {
        Characters.selectedCharacter.EquipArmorPiece(armorPiece);
        Characters.selectedCharacter.equipSlot[armorPiece.armorType].image.sprite = armorPiece.image;
        Characters.selectedCharacter.equipSlot[armorPiece.armorType].item = armorPiece;
        Characters.selectedCharacter.equipSlot[armorPiece.armorType].gameObject.SetActive(true);
        Characters.selectedCharacter.equipSlot[armorPiece.armorType].image.color = Color.white;
    }


    public void OnMenuSellButtonClick()
    {
        //Sell the item
        StartCoroutine(SellItem(inventory.currentInventorySlot));
    }


    IEnumerator SellItem(InventorySlot inventorySlot)
    {
        string data = string.Empty;

        //Get all the data into a string
        data += GameManager.playerID + "|";
        data += Characters.selectedCharacter.id + "|";
        data += inventorySlot.category + "|";
        data += inventorySlot.slotNum + "|";
        data += itemStack.stackCount;



        //Send the data and download the results
        WWWForm form = new WWWForm();
        form.AddField("Data", Encryption.Encrypt(data));
        WWW www = new WWW(GameManager.phpURL + "Sell_Item.php", form);
        yield return www;


        string decryptString = Encryption.Decrypt(www.text);

        //Invalid field was entered
        if (decryptString == "INVALID FIELD")
        {
            Debug.Log("Invalid field");
            yield break;
        }

        //Decryption error
        else if (decryptString == "DECRYPTION ERROR")
        {
            Debug.Log("Decryption error");
            yield break;
        }

        //Split the string up into a string array
        string[] stringArray = decryptString.Split("|"[0]);

        //Assign the stack count and currency
        int stackCount = int.Parse(stringArray[0]);
        Characters.selectedCharacter.currency = (float)System.Math.Round(float.Parse(stringArray[0]), 2);


        //If stack count is 0, remove from the inventory
        if (stackCount == 0)
        {
            Destroy(Characters.selectedCharacter.inventorySlot[inventorySlot.category + "_" + inventorySlot.slotNum].gameObject);
            Characters.selectedCharacter.inventorySlot.Remove(inventorySlot.category + "_" + inventorySlot.slotNum);

            //If the item is an armor piece, delete the equip menu
            if (inventorySlot.item is Armor)
            {
                Destroy(inventorySlot.equipMenuButton.gameObject);
            }
        }
        else
        //Update the inventory slot with the new stack count
        {
            Characters.selectedCharacter.inventorySlot[inventorySlot.category + "_" + inventorySlot.slotNum].stackCount = stackCount;
        }

        inventory.currentInventorySlot = null;
        gameObject.SetActive(false);

    }

}
