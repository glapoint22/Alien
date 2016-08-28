using UnityEngine;
using System.Collections;


public class Startup : MonoBehaviour
{
    IEnumerator Start()
    {
        AssetBundles assetBundles = new AssetBundles();

        //Load the space background
        yield return assetBundles.LoadGameObjectFromAssetBundle("space_background", "Space Background");
        DontDestroyOnLoad(assetBundles.asset);

        //Download the asset bundles
        yield return assetBundles.DownloadAssetBundles();

        //Load the login scene
        yield return assetBundles.LoadScene("scenes/login", "Login", false);
    }
}
