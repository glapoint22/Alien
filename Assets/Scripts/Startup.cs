using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Startup : MonoBehaviour
{
    [SerializeField]
    GameObject spaceBackgroundPrefab;

    [SerializeField]
    private Version version;
    private enum Version { Development, Live}

    IEnumerator Start()
    {
        if(version == Version.Live)
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
        else
        {
            GameObject spaceBackground = Instantiate(spaceBackgroundPrefab);
            DontDestroyOnLoad(spaceBackground);
            SceneManager.LoadScene(1);
        }
    }
}


