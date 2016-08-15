using UnityEngine;


public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Transform[] inventoryCategory;

    [SerializeField]
    private InventorySlotMenu inventorySlotMenu;

    [SerializeField]
    private InventorySlot inventorySlotPrefab;


    [System.NonSerialized]
    public InventorySlot currentInventorySlot;

    [SerializeField]
    private EquipSlots equipSlots;

    public void Build()
    {
        if (GameData.characterInventory == null) return;

        for (int i = 0; i < Characters.character.Count; i++)
        {
            //Populate the character's inventory
            bool isSet = false;
            for (int j = 0; j < GameData.characterInventory.Length; j += 5)
            {
                int index = int.Parse(GameData.characterInventory[j]);

                if (index == Characters.character[i].id)
                {
                    //Create the slot
                    Item item = Shop.shopSlot[GameData.characterInventory[j + 1]].item;
                    int category = int.Parse(GameData.characterInventory[j + 2]);
                    int slotNum = int.Parse(GameData.characterInventory[j + 3]);
                    int stackCount = int.Parse(GameData.characterInventory[j + 4]);
                    InventorySlot inventorySlot = CreateSlot(item, category, slotNum, stackCount, Characters.character[i].id);

                    //If this character is not the current character then disable the slot
                    if (Characters.character[i] != Characters.selectedCharacter)
                    {
                        inventorySlot.gameObject.SetActive(false);
                    }

                    if(!Characters.character[i].inventorySlot.ContainsKey(category + "_" + slotNum))
                    {
                        Characters.character[i].inventorySlot.Add(category + "_" + slotNum, inventorySlot);
                    }
                    
                    isSet = true;
                }
                else
                {
                    if (isSet) break;
                }
            }
        }

    }



    public void UpdateSlots(string[] inventoryData, Item item)
    {
        for (int i = 0; i < inventoryData.Length; i += 3)
        {
            //Set the variables
            int category = int.Parse(inventoryData[i]);
            int slotNum = int.Parse(inventoryData[i + 1]);
            int stackCount = int.Parse(inventoryData[i + 2]);

            //If the item is stackable and already exists in the inventory
            if (Characters.selectedCharacter.inventorySlot.ContainsKey(category + "_" + slotNum))
            {
                InventorySlot inventorySlot = Characters.selectedCharacter.inventorySlot[category + "_" + slotNum];
                inventorySlot.stackCount = stackCount;
            }
            else
            //The item is not stackable so create the new inventory slot
            {
                InventorySlot inventorySlot = CreateSlot(item, category, slotNum, stackCount, Characters.selectedCharacter.id);

                if(!Characters.selectedCharacter.inventorySlot.ContainsKey(category + "_" + slotNum))
                {
                    Characters.selectedCharacter.inventorySlot.Add(category + "_" + slotNum, inventorySlot);
                }
                
            }
        }
    }


    InventorySlot CreateSlot(Item item, int category, int slotNum, int stackCount, int characterID)
    {
        //Create the slot
        InventorySlot inventorySlot = Instantiate(inventorySlotPrefab);
        inventorySlot.item = item;
        inventorySlot.stackCount = stackCount;
        inventorySlot.category = category;
        inventorySlot.slotNum = slotNum;

        //Set the position
        inventorySlot.transform.SetParent(inventoryCategory[category].GetChild(slotNum), false);


        //If the current item is an armor piece
        if (item is Armor)
        {
            inventorySlot.equipMenuButton = equipSlots.CreateEquipMenuButton((Armor)item, inventorySlot, characterID);
        }

        //Set the OnClick fucntion
        inventorySlot.button.onClick.AddListener(delegate { OnInventorySlotButtonClick(inventorySlot); });

        return inventorySlot;
    }


    void OnInventorySlotButtonClick(InventorySlot inventorySlot)
    {
        //Enable or disable the menu
        if (currentInventorySlot != inventorySlot)
        {
            currentInventorySlot = inventorySlot;
            //Set the position of the menu
            inventorySlotMenu.transform.position = inventorySlot.transform.position + new Vector3(inventorySlot.rectTransform.rect.width, 0, 0);

            //Set the menu buttons
            inventorySlotMenu.SetMenuButtons(inventorySlot);


            //Enable the menu
            inventorySlotMenu.gameObject.SetActive(true);
        }
        else
        {
            //Disable the menu
            currentInventorySlot = null;
            inventorySlotMenu.gameObject.SetActive(false);
        }
    }
}
