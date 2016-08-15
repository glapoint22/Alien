using UnityEngine;
using System.Collections.Generic;

public class Item
{
    public string id;
    public string name;
    public double cost;
    public double sell;
    public bool isStackable;
    public int stackCap;
    public int stackCount = 1;
    public int shopCategory;
    public Sprite image;
    public GameObject model;
    public List<Modification> modifications = new List<Modification>();
    public string description;


    protected void SetProperties(string[] properties, Sprite[] images, string[] modifications)
    {
        id = properties[1];
        name = properties[2];
        cost = float.Parse(properties[3]);
        sell = float.Parse(properties[4]);
        shopCategory = int.Parse(properties[5]);
        image = images[int.Parse(properties[6])];
        isStackable = System.Convert.ToBoolean(int.Parse(properties[7]));
        description = "This is the description for this item!";

        if (isStackable)
        {
            stackCount = int.Parse(properties[8]);
            stackCap = int.Parse(properties[9]);
        }

        //Set the modifications for this item
        SetModifications(modifications);
    }


    void SetModifications(string[] itemModifications)
    {
        for (int i = 0; i < itemModifications.Length; i += 6)
        {
            string itemID = itemModifications[i];

            if (itemID == id)
            {
                Modification modification = new Modification();

                modification.name = itemModifications[i + 1];
                modification.description = itemModifications[i + 2];
                modification.upgradeCount = int.Parse(itemModifications[i + 3]);
                modification.baseCost = float.Parse(itemModifications[i + 4]);
                modification.costExponent = float.Parse(itemModifications[i + 5]);

                modifications.Add(modification);

            }
        }
    }
}

