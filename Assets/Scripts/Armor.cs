using UnityEngine;

public class Armor: Item
{
    public SkinnedMeshRenderer[] skinnedMeshRenderer = new SkinnedMeshRenderer[10];
    public int armorType;
    public int armor;
    public int vigor;
    public int resilience;
    public int intelligence;
    public int agility;
    public int vitality;
    public int health;
    public int armorID;


    public void SetProperties(string[] properties, Sprite[] images, ArmorModel[] models, string[] modifications)
    {
        //Set the base properties
        SetProperties(properties, images, modifications);

        //Set the other properties
        armorType = int.Parse(properties[8]);
        armor = int.Parse(properties[9]);
        intelligence = int.Parse(properties[10]);
        agility = int.Parse(properties[11]);
        vigor = int.Parse(properties[12]);
        resilience = int.Parse(properties[13]);
        vitality = int.Parse(properties[14]);
        health = int.Parse(properties[15]);
        armorID = int.Parse(properties[16]);

        for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            skinnedMeshRenderer[i] = models[armorID].raceArmor[i].GetComponent<SkinnedMeshRenderer>();
        }
    }
}
