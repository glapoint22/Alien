using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scene : MonoBehaviour {
    private static EventSystem system;
    public Prompt prompt;

    public static GameObject currentSelectedGameObject = null;


    private bool sixthAxisPressed;
    private bool seventhAxisPressed;
    private enum SelectableDirection { Down, Up, Left, Right }
    private GameObject defaultSelectedGameObject;


    public void SetScene(Groups sceneGroup, int colorIndex, GameObject focusGameObject)
    {
        //Initialize the prompt
        prompt = Prompt.instance;
        prompt.Initialize(sceneGroup);
        UIGroups.SetColor(Groups.Prompt, colorIndex, true);


        //Set the event system
        defaultSelectedGameObject = focusGameObject;
        system = EventSystem.current;
        currentSelectedGameObject = defaultSelectedGameObject.GetComponent<Selectable>().gameObject;
        system.SetSelectedGameObject(currentSelectedGameObject, new BaseEventData(system));
        currentSelectedGameObject.GetComponent<UIEvent>().down = true;

        //Fade in the scene
        UIGroups.SetColor(sceneGroup, colorIndex, true);
        StartCoroutine(UIGroups.Fade(sceneGroup, 0, 1, 0.5f));
    }



    void Update()
    {

        //Deselect the current game object if it has lost focus
        if(system.currentSelectedGameObject == null)
        {
            //currentSelectedGameObject.GetComponent<UIEvent>().OnGameObjectDeselect();
            SetSelectedGameObject();
        }



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
                    SetSelectableGameObject(SelectableDirection.Up);
                }
                else
                {
                    SetSelectableGameObject(SelectableDirection.Down);
                }

            //Down arrorw
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetSelectableGameObject(SelectableDirection.Down);
            }

            //Up arrorw
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetSelectableGameObject(SelectableDirection.Up);
            }

            //Left arrorw
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetSelectableGameObject(SelectableDirection.Left);
            }

            //Right arrorw
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetSelectableGameObject(SelectableDirection.Right);
            }

            //6th Axis Left
            else if (Input.GetAxisRaw("6th Axis") == -1)
            {
                if (!sixthAxisPressed)
                {
                    sixthAxisPressed = true;
                    SetSelectableGameObject(SelectableDirection.Left);
                }
            }

            //6th Axis Right
            else if (Input.GetAxisRaw("6th Axis") == 1)
            {
                if (!sixthAxisPressed)
                {
                    sixthAxisPressed = true;
                    SetSelectableGameObject(SelectableDirection.Right);
                }
            }

            //7th Axis Up
            else if (Input.GetAxisRaw("7th Axis") == 1)
            {
                if (!seventhAxisPressed)
                {
                    seventhAxisPressed = true;
                    SetSelectableGameObject(SelectableDirection.Up);
                }
            }

            //7th Axis Down
            else if (Input.GetAxisRaw("7th Axis") == -1)
            {
                if (!seventhAxisPressed)
                {
                    seventhAxisPressed = true;
                    SetSelectableGameObject(SelectableDirection.Down);
                }
            }
            SetSelectedGameObject();
        }
    }


    public static void SetSelectedGameObject(bool highlightSelected = true)
    {
        system.SetSelectedGameObject(currentSelectedGameObject, new BaseEventData(system));
        if(highlightSelected)currentSelectedGameObject.GetComponent<UIEvent>().OnGameObjectSelect();
    }

    private void SetSelectableGameObject(SelectableDirection selectableDirection)
    {
        Selectable currentSelected = GetCurrentSelected(selectableDirection);

        if (currentSelected.interactable)
        {
            currentSelectedGameObject = currentSelected.gameObject;
        }
        else
        {
            while (!currentSelected.interactable)
            {
                system.SetSelectedGameObject(currentSelected.gameObject, new BaseEventData(system));
                currentSelected = GetCurrentSelected(selectableDirection);
            }
            currentSelectedGameObject = currentSelected.gameObject;
        }
    }


    private Selectable GetCurrentSelected(SelectableDirection selectableDirection)
    {
        Selectable selected = null;

        //If no current game object has the focus than select the default game object
        if (system.currentSelectedGameObject == null)
        {
            return defaultSelectedGameObject.GetComponent<Selectable>();
        }

        switch (selectableDirection)
        {
            case SelectableDirection.Down:
                selected = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                break;
            case SelectableDirection.Up:
                selected = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                break;
            case SelectableDirection.Left:
                selected = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft();
                break;
            case SelectableDirection.Right:
                selected = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight();
                break;
        }
        return selected;
    }
}
