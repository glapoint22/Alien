using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class UIGroups : UI
{
    public static UIGroup[] uiGroup = new UIGroup[Enum.GetNames(typeof(Groups)).Length];
    private static UITheme[] uiTheme;
    private static UIGroups instance;
    private static IEnumerator fadeIn;
    private static IEnumerator fadeOut;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        instance = this;

        uiTheme = GameObject.Find("UI Themes").GetComponent<UIThemes>().uiTheme;

        for(int i = 0; i < uiGroup.Length; i++)
        {
            uiGroup[i].elements = new List<UIGraphic>();
        }
    }

    public static IEnumerator FadeIn(Groups group, float fadeTime)
    {
        //Stop the fade in if it is already in progress
        if (fadeIn != null) instance.StopCoroutine(fadeIn);

        //Start the fade in
        fadeIn = instance.Fade(group, 0, 1, fadeTime);
        yield return fadeIn;

        //Stop the fade in
        instance.StopCoroutine(fadeIn);
    }

    

    public static IEnumerator FadeOut(Groups group, float minAlpha, float fadeTime)
    {
        //Stop the fade out if it is already in progress
        if (fadeOut != null) instance.StopCoroutine(fadeOut);

        //Start the fade out
        fadeOut = instance.Fade(group, minAlpha, -1, fadeTime);
        yield return fadeOut;

        //Stop the fade out
        instance.StopCoroutine(fadeOut);
    }


    private IEnumerator Fade(Groups group, float minAlpha, int direction, float fadeTime)
    {
        float time = 0;
        int index = (int)group;
        int elementCount = uiGroup[index].elements.Count;
        float[] speed = new float[elementCount];
        float[] maxAlpha = new float[elementCount];
        float currentAlpha;

        while (time < fadeTime)
        {
            time += 1 * Time.deltaTime;

            //Loop through all the elements
            for (int i = 0; i < elementCount; i++)
            {
                //Calcualte the speed and get the max alpha for this element
                if (speed[i] == 0)
                {
                    //Test to see which alpha we are using
                    GameObject elementGameObject = uiGroup[index].elements[i].transform.parent.gameObject;
                    float alpha;
                    
                    if (elementGameObject == Scene.currentSelectedGameObject)
                    {
                        UIInteractiveGraphic element = (UIInteractiveGraphic)uiGroup[index].elements[i];
                        alpha = element.selectAlpha;
                    }
                    else if(elementGameObject.tag == "Selectable")
                    {
                        Selectable selectable = elementGameObject.GetComponent<Selectable>();
                        if (!selectable.interactable)
                        {
                            UIInteractiveGraphic element = (UIInteractiveGraphic)uiGroup[index].elements[i];
                            alpha = element.disabledAlpha;
                        }
                        else
                        {
                            alpha = uiGroup[index].elements[i].alpha;
                        }
                    }
                    else
                    {
                        alpha = uiGroup[index].elements[i].alpha;
                    }
                    maxAlpha[i] = direction == -1 ? 1 : alpha;


                    //Get the distance between the current alpha and the alpha we need to get to
                    float distance;
                    if (direction == 1)
                    {
                        distance = maxAlpha[i] - uiGroup[index].elements[i].graphic.color.a;
                    }
                    else
                    {
                        distance = uiGroup[index].elements[i].graphic.color.a - minAlpha;
                    }

                    //Calculate the speed
                    speed[i] = distance / fadeTime;
                }

                //Calculate and assign the current alpha for this element
                currentAlpha = Mathf.Clamp(uiGroup[index].elements[i].graphic.color.a + (speed[i] * direction * Time.deltaTime), minAlpha, maxAlpha[i]);
                uiGroup[index].elements[i].graphic.color = GetUIColor(uiGroup[index].elements[i].graphic.color, currentAlpha);
            }

            yield return null;
        }

    }

    public static void SetColor(Groups group, int themeIndex, bool isTransparent = false)
    {
        int index = (int)group;
        int elementCount = uiGroup[index].elements.Count;
        for (int i = 0; i < elementCount; i++)
        {
            float alpha = isTransparent ? uiGroup[index].elements[i].graphic.color.a : uiGroup[index].elements[i].alpha;
            uiGroup[index].elements[i].graphic.color = GetUIColor(uiTheme[themeIndex].uiColor[uiGroup[index].elements[i].colorIndex], alpha);
        }
    }

    public static IEnumerator TransitionColor(Groups group, int themeIndex, float speed)
    {
        float lerpTime = 0;
        bool running = true;
        int index = (int)group;
        int elementCount = uiGroup[index].elements.Count;

        while (running)
        {
            lerpTime += speed * Time.deltaTime;

            for (int i = 0; i < elementCount; i++)
            {
                uiGroup[index].elements[i].graphic.color = Color.Lerp(uiGroup[index].elements[i].graphic.color, GetUIColor(uiTheme[themeIndex].uiColor[uiGroup[index].elements[i].colorIndex], uiGroup[index].elements[i].graphic.color.a), lerpTime);
                if (uiGroup[index].elements[i].graphic.color == uiTheme[themeIndex].uiColor[uiGroup[index].elements[i].colorIndex] && i == elementCount - 1) running = false;
            }

            yield return null;
        }
    }
}


public struct UIGroup
{
    public List<UIGraphic> elements;
}

public enum Groups { None, ProgressBar, Login, Prompt }
