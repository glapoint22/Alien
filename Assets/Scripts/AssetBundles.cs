using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AssetBundles {
    public GameObject asset;
    public List<string> variants = new List<string>();
    private ProgressBar progressBar;
    private Dictionary<string, int> assetBundleVersion = new Dictionary<string, int>();
    private List<string> assetBundlesToDownload = new List<string>();
    private AssetBundleManifest manifest;

    public IEnumerator GetAssetBundleVersions()
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

    public IEnumerator LoadGameObjectFromAssetBundle(string assetBundleName, string assetName)
    {
        string[] dependencies = manifest.GetAllDependencies(assetBundleName);
        List<AssetBundle> bundles = new List<AssetBundle>();
        int version;
        WWW www;

        //Get the dependencies
        for (int i = 0; i < dependencies.Length; i++)
        {
            string dependency = dependencies[i];
            if (dependencies[i].IndexOf(".") != -1)
            {
                dependency = GetVariant(dependency);
            }

            version = assetBundleVersion[dependency];
            www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + dependency, version);
            yield return www;
            if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

            bundles.Add(www.assetBundle);

        }

        version = assetBundleVersion[assetBundleName];
        www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + assetBundleName, version);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

        //Get the asset from this asset bundle
        AssetBundle bundle = www.assetBundle;
        AssetBundleRequest request = bundle.LoadAssetAsync(assetName);
        yield return request;

        //Instantiate the game object
        asset = UnityEngine.Object.Instantiate((GameObject)request.asset);

        //Unload from memory
        bundle.Unload(false);
        for(int i = 0; i < bundles.Count; i++)
        {
            bundles[i].Unload(false);
        }
    }


    private void GetAssetBundlesToDownload()
    {
        string[] assetBundles;

        //Get all the asset bundles from the manifest
        assetBundles = manifest.GetAllAssetBundles();

        //Find out which asset bundles we need to download based on its version
        for (int i = 0; i < assetBundles.Length; i++)
        {
            //Don't downlaod any variants
            if (assetBundles[i].IndexOf(".") != -1) continue;

            //Check to see if this assetbundle has dependencies
            string[] dependencies = manifest.GetAllDependencies(assetBundles[i]);

            //loop through the dependencies and only set for download if they are variants
            for (int j = 0; j < dependencies.Length; j++)
            {
                if(dependencies[j].IndexOf(".") != -1)
                {
                    SetAssetBundleForDownload(GetVariant(dependencies[j]));
                }
            }

            SetAssetBundleForDownload(assetBundles[i]);
        }
    }

    private void SetAssetBundleForDownload(string assetBundle)
    {
        int version = assetBundleVersion[assetBundle];
        bool isCached = Caching.IsVersionCached(GameManager.assetBundlesURL + assetBundle, version);

        if (!isCached)
        {
            if (!assetBundlesToDownload.Contains(assetBundle))
            {
                assetBundlesToDownload.Add(assetBundle);
            }
        }
    }


    public IEnumerator DownloadAssetBundles()
    {
        //Load the progress bar
        yield return LoadGameObjectFromAssetBundle("progressbar", "Progress Bar");
        progressBar = asset.transform.GetComponent<ProgressBar>();
        progressBar.transform.SetParent(GameObject.Find("Canvas").transform, false);

        //Set the color of the progress bar
        UIGroups.SetColor(Groups.ProgressBar, 2, true);


        //Get the asset bundles that need to be downloaded
        GetAssetBundlesToDownload();

        if (assetBundlesToDownload.Count > 0)
        {
            //Fade in the progress bar
            yield return UIGroups.FadeIn(Groups.ProgressBar, 0.5f);

            //Download the asset bundles
            yield return Download();

            //Wait to fade
            yield return new WaitForSeconds(1);

            //Fade out the progress bar
            yield return UIGroups.FadeOut(Groups.ProgressBar, 0, 0.5f);

            progressBar.progress = 0;
        }
    }

    private IEnumerator Download()
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

    public IEnumerator LoadScene(string assetBundleName, string sceneName, bool showProgress = true)
    {
        float totalProgress = 0;
        string[] dependencies = manifest.GetAllDependencies(assetBundleName);
        int version;
        WWW www;


        //Fade in the progress bar
        if(showProgress) yield return UIGroups.FadeIn(Groups.ProgressBar, 0.5f);


        //Get all the dependencies for this scene
        for (int i = 0; i < dependencies.Length; i++)
        {
            //Get the current dependency
            version = assetBundleVersion[dependencies[i]];
            www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + dependencies[i], version);
            while (!www.isDone)
            {
                if (showProgress)
                {
                    totalProgress += www.progress;
                    progressBar.progress = totalProgress / (dependencies.Length + 1);
                }
                yield return www;
            }
            if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);
        }


        //Get the scene to load from cache
        version = assetBundleVersion[assetBundleName];
        www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + assetBundleName, version);
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

        //Load the scene
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            if (showProgress)
            {
                totalProgress += async.progress;
                progressBar.progress = totalProgress / (dependencies.Length + 1);
            }
            yield return async;
        }
    }

    private string GetVariant(string assetBundleName)
    {
        string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
        string[] split = assetBundleName.Split('.');
        int index = -1;

        // Loop all the assetBundles with variant to find the variant to use
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0]) continue;

            if (variants.Contains(curSplit[1]))
            {
                index = i;
                break;
            }
        }

        
        //Return the variant
        if (index != -1)
        {
            return bundlesWithVariant[index];
        }
        else
        {
            return assetBundleName;
        }
    }

    public IEnumerator GetManifest()
    {
        //Get the manifest
        WWW www = new WWW(GameManager.assetBundlesURL + "AssetBundles");
        yield return www;
        if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

        manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));
    }
}
