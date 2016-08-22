using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Startup : MonoBehaviour
{
    [SerializeField]
    private ProgressBar progressBar;
    private List<string> assetBundlesToDownload = new List<string>();
    private enum Versions { Development, Live }
    [SerializeField]
    private Versions version;

    private Dictionary<string, int> assetBundleVersion = new Dictionary<string, int>();
    private AssetBundleRequest request;
    private AssetBundle bundle;

    IEnumerator Start()
    {
        if(version == Versions.Live)
        {
            //Get all the asset bundle versions
            yield return GetAssetBundleVersions();

            //Progress bar
            yield return GetGameObjectFromAssetBundle("progressbar", "Progress Bar");
            progressBar = Instantiate((GameObject)request.asset).transform.GetComponent<ProgressBar>();
            progressBar.transform.SetParent(GameObject.Find("Canvas").transform, false);
            
            //Unload the current asset bundle
            bundle.Unload(false);


            //Space
            yield return GetGameObjectFromAssetBundle("space_background", "Space Background");
            Instantiate((GameObject)request.asset);

            //Unload the current asset bundle
            bundle.Unload(false);

            //Get the asset bundles that need to be downloaded
            yield return GetAssetBundlesToDownload();
            if (assetBundlesToDownload.Count > 0)
            {
                //Set the color of the progress bar
                UIGroups.SetColor(Groups.ProgressBar, 2, true);

                //Fade in the progress bar
                yield return UIGroups.FadeIn(Groups.ProgressBar, 0.5f);

                //Download the asset bundles
                yield return DownloadAssetBundles();

                //Wait to fade
                yield return new WaitForSeconds(1);

                //Fade out the progress bar
                yield return UIGroups.FadeOut(Groups.ProgressBar, 0, 0.5f);
            }
        }
    }

    IEnumerator GetGameObjectFromAssetBundle(string assetBundleName, string GameObjectName)
    {
        int version = assetBundleVersion[assetBundleName];
        WWW www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + assetBundleName, version);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);


        //Load all assets from this asset bundle
        bundle = www.assetBundle;
        request = bundle.LoadAssetAsync(GameObjectName);
        yield return request;
    }

    IEnumerator GetAssetBundleVersions()
    {
        WWW www = new WWW(GameManager.phpURL + "Get_AssetBundles.php");
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

        //Decrypt
        string decryptData = Encryption.Decrypt(www.text);

        string[] assetBundleVersions = decryptData.Split("|"[0]);

        //Add the name of the asset bundle and its version to the dictionary
        for (int i = 0; i < assetBundleVersions.Length - 1; i += 2)
        {
            assetBundleVersion.Add(assetBundleVersions[i], int.Parse(assetBundleVersions[i + 1]));
        }

    }

    IEnumerator GetAssetBundlesToDownload()
    {
        string[] assetBundles;

        //Get the manifest
        WWW www = new WWW(GameManager.assetBundlesURL + "AssetBundles");
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);


        //Get all the asset bundles from the manifest
        AssetBundleManifest manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));
        assetBundles = manifest.GetAllAssetBundles();


        //Find out which asset bundles we need to download based on its version
        for (int i = 0; i < assetBundles.Length; i++)
        {
            int version = assetBundleVersion[assetBundles[i]];

            bool isCached = Caching.IsVersionCached(GameManager.assetBundlesURL + assetBundles[i], version);

            if (!isCached)
            {
                assetBundlesToDownload.Add(assetBundles[i]);
            }
        }
    }

    IEnumerator DownloadAssetBundles()
    {
        //Loop through all the assetbundles that need to be downloaded
        float totalProgress = 0;
        for (int i = 0; i < assetBundlesToDownload.Count; i++)
        {
            //Display which assetBundle is being downloaded
            progressBar.description.text = "Downloading " + assetBundlesToDownload[i];


            //Download the current assetBundle and display the progress
            int version = assetBundleVersion[assetBundlesToDownload[i]];
            WWW www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + assetBundlesToDownload[i], version);
            while (!www.isDone)
            {
                totalProgress += www.progress;
                progressBar.progress = totalProgress / assetBundlesToDownload.Count;
                
                yield return www;
            }

            totalProgress = i + 1;
            progressBar.progress = totalProgress / assetBundlesToDownload.Count;

            if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

            //Unload the current assetbundle from memory
            www.assetBundle.Unload(false);
        }
    }
}
