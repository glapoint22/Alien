using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the colors for each theme. Color themes are defined in the inspector for each team.
/// </summary>
public class UIThemes : UI
{
    //Public
    public UITheme[] uiTheme;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}


[System.Serializable]
public struct UITheme
{
    public Color[] uiColor;
}



