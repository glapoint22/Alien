using UnityEngine;
using System.Collections.Generic;


public class Character
{
    public string name;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Dictionary<string, InventorySlot> inventorySlot = new Dictionary<string, InventorySlot>();
    public Armor[] armor = new Armor[8];
    public EquipSlot[] equipSlot = new EquipSlot[8];
    public Race race = new Race();
    public int rating;
    public float currency;
    public bool keybindsAreCharacterSpecific;
    public int level;
    public int[] statistics;
    public int health;
    public int id;
    public CharacterSelectButton characterSelectButton;

    private int maxAtlasTextureSize = 512;

    public void EquipArmorPiece(Armor armorPiece)
    {
        //Place the armor piece in the correct slot based on its armor type
        armor[armorPiece.armorType] = armorPiece;

        //Update the armor
        UpdateArmor();
    }


    public void UnequipArmor(Armor armorPiece)
    {
        //Set the armor slot to the base armor
        armor[armorPiece.armorType] = race.baseArmor[armorPiece.armorType];

        //Set the equip slot
        equipSlot[armorPiece.armorType].image.sprite = null;

        if (equipSlot[armorPiece.armorType].equipSlotMenu.childCount > 1)
        {
            equipSlot[armorPiece.armorType].UnequipMenuButton.gameObject.SetActive(false);
        }
        else
        {
            equipSlot[armorPiece.armorType].equipSlotMenu.gameObject.SetActive(false);

        }


            
        equipSlot[armorPiece.armorType].image.color = new Color(1, 1, 1, 0);

        //Update the armor
        UpdateArmor();
    }


    public void UpdateArmor()
    {
        //A list of all armor meshes that will be combined into one
        List<CombineInstance> meshes = new List<CombineInstance>();

        //An array for all the armor textures
        Texture2D[] textures = new Texture2D[armor.Length];


        for (int i = 0; i < armor.Length; i++)
        {
            //Get the texture from this armor piece
            textures[i] = (Texture2D)armor[i].skinnedMeshRenderer[race.ID].sharedMaterial.mainTexture;

            //Add each armor piece to the meshes list so they can be combined
            if (armor[i].skinnedMeshRenderer[race.ID].sharedMesh != null)
            {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = armor[i].skinnedMeshRenderer[race.ID].sharedMesh;
                combineInstance.transform = armor[i].skinnedMeshRenderer[race.ID].transform.localToWorldMatrix;
                meshes.Add(combineInstance);
            }
        }


        //Get the bindposes from the character's skinned mesh renderer
        Matrix4x4[] bindposes;
        bindposes = skinnedMeshRenderer.sharedMesh.bindposes;


        //Combine all the armor meshes into one mesh
        skinnedMeshRenderer.sharedMesh = new Mesh();
        skinnedMeshRenderer.sharedMesh.CombineMeshes(meshes.ToArray(), true, true);

        //Combine all the armor textures into one atlas texture
        Texture2D atlas = new Texture2D(maxAtlasTextureSize, maxAtlasTextureSize);
        atlas.PackTextures(textures, 0, maxAtlasTextureSize);

        //Assign the atlas texture to the material
        Material armorMaterial = new Material(Shader.Find("Standard"));
        armorMaterial.mainTexture = atlas;


        //Figure out the length for the boneWeights array
        int length = 0;
        for (int i = 0; i < meshes.Count; i++)
        {
            length += meshes[i].mesh.boneWeights.Length;
        }

        BoneWeight[] boneWeights = new BoneWeight[length];



        //Copy all the bone weight information from each armor mesh to the boneWeights array
        int startIndex = 0;
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].mesh.boneWeights.CopyTo(boneWeights, startIndex);
            startIndex += meshes[i].mesh.boneWeights.Length;
        }


        //Assign al the variables to the character's skinned mesh renderer
        skinnedMeshRenderer.sharedMaterial = armorMaterial;
        skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
        skinnedMeshRenderer.sharedMesh.bindposes = bindposes;
        skinnedMeshRenderer.sharedMesh.RecalculateBounds();
    }


    public void SetBaseArmor(RaceModel[] raceModels)
    {
        for (int i = 0; i < race.baseArmor.Length; i++)
        {
            race.baseArmor[i] = new Armor();
            race.baseArmor[i].armorType = i;
            race.baseArmor[i].skinnedMeshRenderer[race.ID] = raceModels[race.ID].baseArmor[i].GetComponent<SkinnedMeshRenderer>();
        }
    }


    public void SetAbilities(int characterID)
    {
        bool isSet = false;
        for (int i = 0; i < GameData.characterAbilities.Length; i += 4)
        {
            int ID = int.Parse(GameData.characterAbilities[i]);

            if (ID == characterID)
            {
                Ability ability = new Ability();
                ability.name = GameData.characterAbilities[i + 1];
                ability.description = GameData.characterAbilities[i + 2];
                ability.availableLevel = int.Parse(GameData.characterAbilities[i + 3]);

                race.abilities.Add(ability);
                isSet = true;
            }
            else
            {
                if (isSet) break;
            }
        }
    }


    public void SetAttributes(int characterID)
    {
        bool isSet = false;
        int attributeIndex = 0;
        for (int i = 0; i < GameData.characterAttributes.Length; i += 3)
        {
            int ID = int.Parse(GameData.characterAttributes[i]);
            if (ID == characterID)
            {
                race.attributes[attributeIndex].baseValue = int.Parse(GameData.characterAttributes[i + 1]);
                race.attributes[attributeIndex].levelExponent = float.Parse(GameData.characterAttributes[i + 2]);
                race.attributes[attributeIndex].CalculateValue(level);

                attributeIndex++;
                isSet = true;
            }
            else
            {
                if (isSet) break;
            }
        }
    }


    public void SetStatistics(int characterID)
    {
        statistics = new int[GameData.statistics.Length];
        bool isSet = false;
        int statisticIndex = 0;
        for (int i = 0; i < GameData.characterStatistics.Length; i += 2)
        {
            int ID = int.Parse(GameData.characterStatistics[i]);
            if (ID == characterID)
            {
                statistics[statisticIndex] = int.Parse(GameData.characterStatistics[i + 1]);
                statisticIndex++;
                isSet = true;
            }
            else
            {
                if (isSet) break;
            }
        }
    }
}
