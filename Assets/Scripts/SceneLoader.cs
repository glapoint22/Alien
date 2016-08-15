using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static GameObject gameObj;

    public Text text;


    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObj = gameObject;
    }

   
    public IEnumerator LoadScene(int sceneIndex)
    {
        transform.GetChild(0).gameObject.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        yield return async;
    }


    

}
