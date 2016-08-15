using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SetMainScene : MonoBehaviour
{
    [SerializeField]
    private AssetBundles assetBundles;

    [SerializeField]
    private GameData gameData;

    [SerializeField]
    private Shop shop;

    [SerializeField]
    private Characters characters;

    [SerializeField]
    private Inventory inventory;

    // Use this for initialization
    IEnumerator Start()
    {
        if(Debug.isDebugBuild)
        {
            if (GameManager.assetBundles == null)
            {
                SceneManager.LoadScene(0);
                yield return null;
            }
        }
        

        //Download data from the database
        yield return StartCoroutine(gameData.DownloadData());

        //Load the assets
        yield return StartCoroutine(assetBundles.LoadAssets());

        //Build the shop
        shop.Build();

        //Setup the characters
        characters.SetCharacters();

        //Build the inventory
        inventory.Build();

        //Disable the loading screen
        if (SceneLoader.gameObj != null) SceneLoader.gameObj.transform.GetChild(0).gameObject.SetActive(false);
    }
}
