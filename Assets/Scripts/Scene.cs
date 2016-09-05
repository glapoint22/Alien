using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scene : MonoBehaviour {
    public EventSystem system;
    public Prompt prompt;

    public static GameObject currentSelectedGameObject = null;


    private bool sixthAxisPressed;
    private bool seventhAxisPressed;


    public void SetScene(Groups sceneGroup, int colorIndex, GameObject focusGameObject)
    {
        //Initialize the prompt
        prompt = Prompt.instance;
        prompt.Initialize(sceneGroup);
        UIGroups.SetColor(Groups.Prompt, colorIndex, true);


        //Set the event system
        system = EventSystem.current;
        currentSelectedGameObject = focusGameObject.GetComponent<Selectable>().gameObject;
        system.SetSelectedGameObject(currentSelectedGameObject.gameObject, new BaseEventData(system));

        //Fade in the scene
        UIGroups.SetColor(sceneGroup, colorIndex, true);
        StartCoroutine(UIGroups.FadeIn(sceneGroup, 0.5f));
    }



    void Update()
    {


        if (Input.GetAxisRaw("6th Axis") == 0)
        {
            sixthAxisPressed = false;
        }

        if (Input.GetAxisRaw("7th Axis") == 0)
        {
            seventhAxisPressed = false;
        }


        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxisRaw("6th Axis") == -1 || Input.GetAxisRaw("6th Axis") == 1 || Input.GetAxisRaw("7th Axis") == -1 || Input.GetAxisRaw("7th Axis") == 1)
        {
            currentSelectedGameObject.gameObject.GetComponent<UIEvent>().OnGameObjectDeselect();
            //Tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject;
                }
                else
                {
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject;
                }

            //Down arrorw
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject;
            }

            //Up arrorw
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject;
            }

            //Left arrorw
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().gameObject;
            }

            //Right arrorw
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().gameObject;
            }

            //6th Axis Left
            else if (Input.GetAxisRaw("6th Axis") == -1)
            {
                if (!sixthAxisPressed)
                {
                    sixthAxisPressed = true;
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().gameObject;
                }
            }

            //6th Axis Right
            else if (Input.GetAxisRaw("6th Axis") == 1)
            {
                if (!sixthAxisPressed)
                {
                    sixthAxisPressed = true;
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().gameObject;
                }
            }

            //7th Axis Up
            else if (Input.GetAxisRaw("7th Axis") == 1)
            {
                if (!seventhAxisPressed)
                {
                    seventhAxisPressed = true;
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().gameObject;
                }
            }

            //7th Axis Down
            else if (Input.GetAxisRaw("7th Axis") == -1)
            {
                if (!seventhAxisPressed)
                {
                    seventhAxisPressed = true;
                    currentSelectedGameObject = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject;
                }
            }



            SelectGameObject();
        }
    }


    void SelectGameObject()
    {
        system.SetSelectedGameObject(currentSelectedGameObject, new BaseEventData(system));
        currentSelectedGameObject.GetComponent<UIEvent>().OnGameObjectSelect();
    }
}
