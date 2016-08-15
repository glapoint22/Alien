using UnityEngine;
using System.Collections.Generic;


public class Shop : MonoBehaviour
{
    public static Dictionary<string, Slot> shopSlot = new Dictionary<string, Slot>();

    [System.NonSerialized]
    public Slot currentShopSlot;

    private Sprite[] icons;
    
    
    private ArmorModel[] armorModels;
    private GameObject[] weaponModels;

    [SerializeField]
    private Transform[] shopCategory;

    [SerializeField]
    private ShopSlotMenu shopSlotMenu;

    [SerializeField]
    private Slot shopSlotPrefab;

    public void Build()
    {
        //Icons
        icons = GameManager.assets["Icons"].GetComponent<Icons>().icons;


        //Armor Models
        armorModels = GameManager.assets["ArmorModels"].GetComponent<ArmorAssets>().armorModels;


        //Weapon Models
        weaponModels = GameManager.assets["WeaponModels"].GetComponent<WeaponAssets>().weaponModels;

        
        //Build weapon slots
        BuildSlots(GameData.weapons, GameData.weaponFieldCount);


        //Build armor slots
        BuildSlots(GameData.armor, GameData.armorFieldCount);
    }


    Item CreateItem(string[] itemProperties)
    {
        //Crate the item
        Item item = null;

        //Get the item type for this item
        ItemType itemType = (ItemType)int.Parse(itemProperties[0]);

        switch (itemType)
        {
            //Weapon
            case ItemType.Weapon:
                //Instantiate the new weapon
                Weapon weapon = new Weapon();

                //Set this item's properties
                weapon.SetProperties(itemProperties, icons, weaponModels, GameData.itemModifications);

                //Set this weapon as the new item
                item = weapon;
                break;
            
            
            //Armor
            case ItemType.Armor:
                //Instantiate the new armor piece
                Armor armorPiece = new Armor();

                //Set this armor's properties
                armorPiece.SetProperties(itemProperties, icons, armorModels, GameData.itemModifications);

                //Set this armor piece as the new item
                item = armorPiece;

                break;
        }
        return item;
    }


    void CreateSlot(string[] itemProperties)
    {
        //Create the slot
        Slot slot = Instantiate(shopSlotPrefab);
        slot.item = CreateItem(itemProperties);
        slot.transform.SetParent(shopCategory[slot.item.shopCategory], false);
        slot.stackCount = slot.item.stackCount;

        //Set the OnClick fucntion
        slot.button.onClick.AddListener(delegate { OnShopSlotButtonClick(slot); });


        //Add this slot to the shopSlot dictionary
        if (!shopSlot.ContainsKey(slot.item.id))
        {
            shopSlot.Add(slot.item.id, slot);
        }
    }


    void BuildSlots(string[] data, int numFields)
    {
        int index = 0;
        for (int i = 0; i < data.Length / numFields; i++)
        {
            //Get the properties from this current item
            string[] itemProperties = new string[numFields];
            System.Array.Copy(data, index, itemProperties, 0, numFields);

            //Create the slot
            CreateSlot(itemProperties);

            //Increment the count of numFields to go to the next set of properties for the next item
            index += numFields;
        }
    }

    void OnShopSlotButtonClick(Slot shopSlot)
    {
        //Enable or disable the menu
        if (currentShopSlot != shopSlot)
        {
            currentShopSlot = shopSlot;
            //Set the position of the menu
            shopSlotMenu.transform.position = shopSlot.transform.position + new Vector3(shopSlot.rectTransform.rect.width, 0, 0);

            //Set the menu buttons
            shopSlotMenu.SetMenuButtons(shopSlot);


            //Enable the menu
            shopSlotMenu.gameObject.SetActive(true);
        }
        else
        {
            //Disable the menu
            currentShopSlot = null;
            shopSlotMenu.gameObject.SetActive(false);
        }
    }
}
