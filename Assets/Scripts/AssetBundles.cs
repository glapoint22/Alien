using UnityEngine;
using System.Collections;
using System;


public class AssetBundles : MonoBehaviour
{
    [SerializeField]
    private string bundlesDirectory;


    // Use this for initialization
    public IEnumerator LoadAssets()
    {
        for(int i = 0; i < GameManager.assetBundles.Length; i++)
        {
            if (GameManager.assetBundles[i].Contains(bundlesDirectory))
            {
                //Get the current asset bundle from cache
                int version = GameManager.assetBundleVersion[GameManager.assetBundles[i]];
                WWW www = WWW.LoadFromCacheOrDownload(GameManager.assetBundlesURL + GameManager.assetBundles[i], version);
                yield return www;
                if (!string.IsNullOrEmpty(www.error)) throw new Exception("WWW download had an error: " + www.error);

                
                //Load all assets from this asset bundle
                AssetBundle bundle = www.assetBundle;
                AssetBundleRequest request = bundle.LoadAllAssetsAsync();
                yield return request;


                //Add the assets to the assets dictionary if it is not already added
                for (int j = 0; j < request.allAssets.Length; j++)
                {
                    if (!GameManager.assets.ContainsKey(request.allAssets[j].name))
                    {
                        GameManager.assets.Add(request.allAssets[j].name, (GameObject)request.allAssets[j]);
                    }
                }

                //Unload the current asset bundle
                bundle.Unload(false);
            }
        }
    }
}
