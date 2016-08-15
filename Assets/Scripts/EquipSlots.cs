using UnityEngine;
using System.Collections;

public class EquipSlots : MonoBehaviour
{
    [SerializeField]
    private GameObject equipMenuButtonPrefab;

    private EquipSlot currentequipSlot;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private InventorySlotMenu inventorySlotMenu;


    public void OnEquipSlotButtonClick(EquipSlot equipSlot)
    {
        //Disable all the equip slot menus
        for (int i = 0; i < Characters.selectedCharacter.armor.Length; i++)
        {
            Characters.selectedCharacter.equipSlot[i].equipSlotMenu.gameObject.SetActive(false);
        }


        //If the current equip slot does not equal this equip slot
        if (currentequipSlot != equipSlot)
        {
            currentequipSlot = null;


            //The equip slot is equipped - show the unequip menu button
            if (equipSlot.button.image.sprite != null)
            {
                equipSlot.equipSlotMenu.gameObject.SetActive(true);
                equipSlot.UnequipMenuButton.gameObject.SetActive(true);
                currentequipSlot = equipSlot;
            }

            //If there are available armor pieces that can be equipped into this slot
            if (equipSlot.equipSlotMenu.childCount > 1)
            {
                equipSlot.equipSlotMenu.gameObject.SetActive(true);
                currentequipSlot = equipSlot;
            }

        }
        else
        //The current equip slot does equal this equip slot
        {
            currentequipSlot = null;
        }
    }


    public void OnMenuUnequipButtonClick(EquipSlot equipSlot)
    {
        StartCoroutine(Unequip(equipSlot));
        currentequipSlot.equipSlotMenu.gameObject.SetActive(false);
        currentequipSlot = null;

    }


    public EquipMenuButton CreateEquipMenuButton(Armor armorPiece, InventorySlot inventorySlot, int characterID)
    {
        int characterIndex = Characters.character.FindIndex(i => i.id == characterID);

        //Instantiate and position the equip menu button
        EquipMenuButton equipMenuButton = Instantiate(equipMenuButtonPrefab).GetComponent<EquipMenuButton>();
        equipMenuButton.transform.SetParent(Characters.character[characterIndex].equipSlot[armorPiece.armorType].equipSlotMenu);
        equipMenuButton.transform.position = Vector3.zero;

        //Add the image
        equipMenuButton.image.sprite = armorPiece.image;

        //Set the OnClick function
        equipMenuButton.button.onClick.AddListener(delegate { OnEquipMenuButtonClick(inventorySlot); });

        //Return the new equipMenuButton
        return equipMenuButton;
    }


    public void OnEquipMenuButtonClick(InventorySlot inventorySlot)
    {
        StartCoroutine(inventorySlotMenu.Equip(inventorySlot));
        currentequipSlot.equipSlotMenu.gameObject.SetActive(false);
        currentequipSlot = null;
    }


    IEnumerator Unequip(EquipSlot equipSlot)
    {
        string data = string.Empty;

        //Get all the data into a string
        data += GameManager.playerID + "|";
        data += Characters.selectedCharacter.id + "|";
        data += equipSlot.item.id;

        //Send the data and download the results
        WWWForm form = new WWWForm();
        form.AddField("Data", Encryption.Encrypt(data));
        WWW www = new WWW(GameManager.phpURL + "Unequip.php", form);
        yield return www;



        //Decrypt the data
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

        //Inventory full
        else if (decryptString == "INVENTORY FULL")
        {
            Debug.Log("Inventory full");
        }
        else
        {
            string[] inventoryData = decryptString.Split("|"[0]);
            inventory.UpdateSlots(inventoryData, equipSlot.item);
            Characters.selectedCharacter.UnequipArmor((Armor)equipSlot.item);
        }
    }

}
