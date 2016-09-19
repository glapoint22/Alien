using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Startup : MonoBehaviour
{
    [SerializeField]
    private GameObject spaceBackgroundPrefab;

    [SerializeField]
    private Version version;
    private enum Version { Development, Live}

    IEnumerator Start()
    {
        if(version == Version.Live)
        {
            AssetBundles assetBundles = new AssetBundles();

            //Variants
            string systemLanguage = Application.systemLanguage.ToString().ToLower();
            assetBundles.variants.Add(systemLanguage);

            //Load the space background
            yield return assetBundles.LoadGameObjectFromAssetBundle("space_background", "Space Background");
            DontDestroyOnLoad(assetBundles.asset);

            //Download the asset bundles
            yield return assetBundles.DownloadAssetBundles();


            string[] dependencies = assetBundles.manifest.GetAllDependencies("localization");
            string dependency = assetBundles.RemapVariantName(dependencies[0]);
            int version = assetBundles.assetBundleVersion[dependency];
            WWW www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + dependency, version);
            yield return www;

            yield return assetBundles.LoadGameObjectFromAssetBundle("localization", "Localization");
            DontDestroyOnLoad(assetBundles.asset);


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


