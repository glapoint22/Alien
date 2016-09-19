using UnityEngine;

public class Localization : MonoBehaviour {
    [SerializeField]
    private TextAsset strings;

    public static LocalizationKey key;

	// Use this for initialization
	void Start () {
        key = JsonUtility.FromJson<LocalizationKey>(strings.text);
    }
}
