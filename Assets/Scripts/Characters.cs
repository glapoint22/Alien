using UnityEngine;
using System.Collections.Generic;

public class Characters : MonoBehaviour
{
    public static List<Character> character = new List<Character>();
    public static Character selectedCharacter;

    [SerializeField]
    private EquipSlot equipSlotPrefab;

    [SerializeField]
    private Transform[] equipSlotParents;

    [SerializeField]
    private Transform characterSelectPanel;

    [SerializeField]
    private CharacterSelectButton characterSelectButtonPrefab;

    [SerializeField]
    private EquipSlots equipSlots;


    [SerializeField]
    private GameObject deleteCharacterPrompt;

    public void SetCharacters()
    {
        character.Clear();

        //Get the race models
        RaceModel[] raceModels = GameManager.assets["RaceModels"].GetComponent<RaceModels>().raceModels;

        int j = 0;
        for(int i = 0; i < GameData.characterCount; i++)
        {
            Character currentCharacter = new Character();
            
            currentCharacter.level = int.Parse(GameData.characters[j]);
            currentCharacter.name = GameData.characters[j + 1];
            currentCharacter.rating = int.Parse(GameData.characters[j + 2]);
            currentCharacter.currency = float.Parse(GameData.characters[j + 3]);
            currentCharacter.race.ID = int.Parse(GameData.characters[j + 4]);
            currentCharacter.race.name = GameData.characters[j + 5];
            currentCharacter.race.teamColor = (TeamColor)System.Enum.Parse(typeof(TeamColor), GameData.characters[j + 6]);
            currentCharacter.keybindsAreCharacterSpecific = System.Convert.ToBoolean(int.Parse(GameData.characters[j + 7]));
            currentCharacter.race.baseHealth = int.Parse(GameData.characters[j + 8]);
            currentCharacter.id = int.Parse(GameData.characters[j + 9]);

            //Instantiate the race model that this character is
            GameObject raceModel = Instantiate(raceModels[currentCharacter.race.ID].race);


            //Set the character's skinnedMeshRenderer
            currentCharacter.skinnedMeshRenderer = raceModel.GetComponent<SkinnedMeshRenderer>();

            //Set the base armor
            currentCharacter.SetBaseArmor(raceModels);


            //Abilities
            currentCharacter.SetAbilities(currentCharacter.id);


            //Attributes
            currentCharacter.SetAttributes(currentCharacter.id);


            //Statistics
            currentCharacter.SetStatistics(currentCharacter.id);

            

            //Equip this character's armor
            for (int k = 0; k < currentCharacter.armor.Length; k++)
            {
                //Create the equip slot
                currentCharacter.equipSlot[k] = Instantiate(equipSlotPrefab);
                currentCharacter.equipSlot[k].transform.SetParent(equipSlotParents[k], false);


                //Set the OnClick fucntion for the equip slot
                EquipSlot equipSlotReference = currentCharacter.equipSlot[k];
                currentCharacter.equipSlot[k].button.onClick.AddListener(delegate { equipSlots.OnEquipSlotButtonClick(equipSlotReference); });


                //Set the Onclick function for the unequip menu button
                currentCharacter.equipSlot[k].UnequipMenuButton.onClick.AddListener(delegate { equipSlots.OnMenuUnequipButtonClick(equipSlotReference); });


                bool found = false;

                //Loop through all the equipped items
                if(GameData.equipped != null)
                {
                    for (int l = 0; l < GameData.equipped.Length; l += 3)
                    {
                        //Get the character index
                        int characterIndex = int.Parse(GameData.equipped[l]);
                        

                        //If the current character matches the character ID, equip the armor piece
                        if (currentCharacter.id == characterIndex && int.Parse(GameData.equipped[l + 2]) == k)
                        {
                            currentCharacter.armor[k] = (Armor)Shop.shopSlot[GameData.equipped[l + 1]].item;
                            currentCharacter.equipSlot[k].image.sprite = currentCharacter.armor[k].image;
                            currentCharacter.equipSlot[k].image.color = Color.white;
                            currentCharacter.equipSlot[k].item = currentCharacter.armor[k];
                            found = true;
                            break;
                        }
                    }
                }
                

                //No armor piece found so equip this character's base armor
                if (!found)
                {
                    currentCharacter.armor[k] = currentCharacter.race.baseArmor[k];
                }

                
            }
            currentCharacter.UpdateArmor();



            //Instantiate the character select button
            currentCharacter.characterSelectButton = Instantiate(characterSelectButtonPrefab);
            currentCharacter.characterSelectButton.transform.SetParent(characterSelectPanel, false);

            //Populate the name, race, and level
            currentCharacter.characterSelectButton.characterName.text = currentCharacter.name;
            currentCharacter.characterSelectButton.race.text = currentCharacter.race.name;
            currentCharacter.characterSelectButton.level.text = currentCharacter.level.ToString();

            //Set the Onclick event
            Character characterReference = currentCharacter;
            currentCharacter.characterSelectButton.button.onClick.AddListener(delegate { OnCharacterSelectButtonClick(characterReference); });


            //Add to the character list
            character.Add(currentCharacter);



            j += 10;

            
        }

        


        //Set the first character to be selected
        character[0].skinnedMeshRenderer.enabled = true;
        selectedCharacter = character[0];

        for(int i = 0; i < selectedCharacter.armor.Length; i++)
        {
            selectedCharacter.equipSlot[i].gameObject.SetActive(true);

        }
    }


    void OnCharacterSelectButtonClick(Character character)
    {
        //Return if the selected character is the current character
        if (character == selectedCharacter) return;

        //Disable the current character skinnedMeshRenderer and enable the selected character's skinnedMeshRenderer
        selectedCharacter.skinnedMeshRenderer.enabled = false;
        character.skinnedMeshRenderer.enabled = true;


        for (int i = 0; i < selectedCharacter.armor.Length; i++)
        {
            //Disable the armor slot for the current character
            selectedCharacter.equipSlot[i].gameObject.SetActive(false);

            //Enable the armor slot for the selected character
            character.equipSlot[i].gameObject.SetActive(true);
        }

        //Disable the current character's inventory slots
        Dictionary<string, InventorySlot>.ValueCollection inventorySlots = selectedCharacter.inventorySlot.Values;
        foreach(InventorySlot inventorySlot in inventorySlots)
        {
            inventorySlot.gameObject.SetActive(false);
        }


        //Enable the selected character's inventory slots
        inventorySlots = character.inventorySlot.Values;
        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            inventorySlot.gameObject.SetActive(true);
        }

        //Assign the current character
        selectedCharacter = character;
    }


    public void OnCreateNewCharacterButtonClick()
    {
        if(SceneLoader.gameObj != null)
        {
            SceneLoader sceneLoader = SceneLoader.gameObj.GetComponent<SceneLoader>();
            StartCoroutine(sceneLoader.LoadScene(2));
        }
    }




    public void OnDeleteCharacterButtonClick()
    {
        deleteCharacterPrompt.SetActive(true);
    }
}
