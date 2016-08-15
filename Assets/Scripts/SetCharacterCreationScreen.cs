using UnityEngine;
using System.Collections;

public class SetCharacterCreationScreen : MonoBehaviour
{
    [SerializeField]
    private CharacterCreation characterCreation;

    [SerializeField]
    private AssetBundles assetBundles;


    IEnumerator Start()
    {
        //Load the assets
        yield return StartCoroutine(assetBundles.LoadAssets());

        //Download data from the database
        yield return StartCoroutine(characterCreation.GetRaceData());

        if (SceneLoader.gameObj != null) SceneLoader.gameObj.SetActive(false);
    }
}
