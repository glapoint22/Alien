using UnityEngine;
using System.Collections;
using System;

public class GameData : MonoBehaviour
{
    public static int weaponFieldCount;
    public static string[] weapons;
    public static string[] itemModifications;
    public static int armorFieldCount;
    public static string[] armor;
    public static int characterCount;
    public static string[] characters;
    public static string[] characterAbilities;
    public static string[] attributes;
    public static string[] characterAttributes;
    public static string[] statistics;
    public static string[] characterStatistics;
    public static string[] characterInventory;
    public static string[] characterItemModifications;
    public static string[] equipped;
    public static int lastCharacterPlayedIndex = 1;

    private string decryptData;

    // Use this for initialization
    public IEnumerator DownloadData()
    {
        
        WWW www;
        int startIndex = 0;
        int index = 0;

        WWWForm form = new WWWForm();

        form.AddField("PlayerID", Encryption.Encrypt(GameManager.playerID));


        //Download the data from the database
        www = new WWW(GameManager.phpURL + "Game_Data.php", form);
        yield return www;


        //Decrypt
        decryptData = Encryption.Decrypt(www.text);



        try
        {
            //Number of weapon fields
            weaponFieldCount = int.Parse(GetData(ref startIndex, ref index)[0]);


            //Weapons
            weapons = GetData(ref startIndex, ref index);


            //Item mods
            itemModifications = GetData(ref startIndex, ref index);


            //Number of armor fields
            armorFieldCount = int.Parse(GetData(ref startIndex, ref index)[0]);


            //Armor
            armor = GetData(ref startIndex, ref index);


            //Number of characters
            characterCount = int.Parse(GetData(ref startIndex, ref index)[0]);



            //Characters
            characters = GetData(ref startIndex, ref index);


            //Character abilities
            characterAbilities = GetData(ref startIndex, ref index);


            //Attributes
            attributes = GetData(ref startIndex, ref index);


            //Character Attributes
            characterAttributes = GetData(ref startIndex, ref index);


            //Statistics
            statistics = GetData(ref startIndex, ref index);

            //Character Statistics
            characterStatistics = GetData(ref startIndex, ref index);


            //Character Inventory
            characterInventory = GetData(ref startIndex, ref index);


            //Character Item Modifications
            characterItemModifications = GetData(ref startIndex, ref index);


            //Equipped
            equipped = GetData(ref startIndex, ref index);
        }
        catch(Exception)
        {
            Debug.Log("Error!");
            yield break;
        }
    }


    string[] GetData(ref int startIndex, ref int index)
    {
        startIndex = index + 1;
        index = decryptData.IndexOf("*", startIndex);
        int length = index - startIndex - 1;

        if (length == 0) return null;

        string data = decryptData.Substring(startIndex, length);
        return data.Split("|"[0]);
    }
}
