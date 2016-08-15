using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DownloadManager : MonoBehaviour
{
    [SerializeField]
    private Image progressBar;

    [SerializeField]
    private Text info;


    IEnumerator Start()
    {
        yield return StartCoroutine(GetAssetBundleVersions());
        yield return StartCoroutine(DownloadAssetBundles());
    }

    IEnumerator GetAssetBundleVersions()
    {
        WWW www = new WWW(GameManager.phpURL + "Get_AssetBundles.php");
        yield return www;

        //Decrypt
        string decryptData = Encryption.Decrypt(www.text);

        string[] assetBundleVersions = decryptData.Split("|"[0]);

        //Add the name of the asset bundle and its version to the dictionary
        for(int i = 0; i < assetBundleVersions.Length - 1; i += 2)
        {
            GameManager.assetBundleVersion.Add(assetBundleVersions[i], int.Parse(assetBundleVersions[i + 1]));
        }

    }


    IEnumerator DownloadAssetBundles()
    {
        //Get the manifest
        WWW www = new WWW(GameManager.assetBundlesURL + "Bundles");
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);


        //Get all the asset bundles from the manifest
        AssetBundleManifest manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));
        GameManager.assetBundles = manifest.GetAllAssetBundles();

        
        //Find out which asset bundles we need to download based on its version
        List<string> assetBundlesToDownload = new List<string>();
        for (int i = 0; i < GameManager.assetBundles.Length; i++)
        {
            int version = GameManager.assetBundleVersion[GameManager.assetBundles[i]];
            

            bool isCached = Caching.IsVersionCached(GameManager.assetBundlesURL + GameManager.assetBundles[i], version);

            if (!isCached)
            {
                assetBundlesToDownload.Add(GameManager.assetBundles[i]);
            }
        }



        //Loop through all the assetbundles that need to be downloaded
        float totalProgress = 0;
        for (int i = 0; i < assetBundlesToDownload.Count; i++)
        {
            //Get the assetBundle name
            int startIndex = assetBundlesToDownload[i].IndexOf("/") + 1;
            string assetBundleName = assetBundlesToDownload[i].Substring(startIndex).Replace("_", " ");

            //Display which assetBundle is being downloaded
            info.text = " Downloading " + assetBundleName;


            //Download the current assetBundle and display the progress
            int version = GameManager.assetBundleVersion[assetBundlesToDownload[i]];
            www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + assetBundlesToDownload[i], version);
            while (!www.isDone)
            {
                totalProgress += www.progress;

                progressBar.fillAmount = totalProgress / assetBundlesToDownload.Count;

                yield return www;
            }

            totalProgress = i + 1;
            progressBar.fillAmount = totalProgress / assetBundlesToDownload.Count;

            if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);


            //Unload the current assetBundle from memory
            www.assetBundle.Unload(false);
        }

        

        //Load the login scene
        SceneManager.LoadScene(1);

    }
}
