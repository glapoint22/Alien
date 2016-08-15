using UnityEngine;
using System.Collections.Generic;

public class Race
{
    public string name;
    public GameObject model;
    public TeamColor teamColor;
    public string description;
    public Armor[] baseArmor = new Armor[8];
    public List<Ability> abilities = new List<Ability>();
    public Attribute[] attributes = new Attribute[5];
    public int baseHealth;
    public int ID;
}
