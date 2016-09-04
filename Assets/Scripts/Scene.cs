using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Scene : MonoBehaviour {
    public EventSystem system;
    public Prompt prompt;

    public static Selectable currentSelectedGameObject = null;

    public IEnumerator SetScene(Groups sceneGroup, int colorIndex, GameObject focusGameObject)
    {
        //Initialize the prompt
        prompt = Prompt.instance;
        prompt.Initialize(sceneGroup);
        UIGroups.SetColor(Groups.Prompt, colorIndex, true);

        //Fade in the scene
        UIGroups.SetColor(sceneGroup, colorIndex, true);
        yield return UIGroups.FadeIn(sceneGroup, 0.5f);


        //Set the event system
        system = EventSystem.current;
        currentSelectedGameObject = focusGameObject.GetComponent<Selectable>();
        SelectGameObject();
    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedGameObject.gameObject.GetComponent<UIEvent>().OnGameObjectDeselect();
            //Tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                }
                else
                {
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                }
            //Down arrorw
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            }
            //Up arrorw
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            }
            //Left arrorw
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft();
            }
            //Left arrorw
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight();
            }
            SelectGameObject();
        }
    }


    void SelectGameObject()
    {
        system.SetSelectedGameObject(currentSelectedGameObject.gameObject, new BaseEventData(system));
        currentSelectedGameObject.GetComponent<UIEvent>().OnGameObjectSelect();
    }
}
