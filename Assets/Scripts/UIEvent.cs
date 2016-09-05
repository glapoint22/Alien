using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIEvent : UIParent, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //Public
    [System.NonSerialized]
    public bool down;

    //Private
    private bool over;
    
    private IEnumerator onMouseUp;

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
        if (!down)
        {
            for(int i = 0; i < children.Length; i++)
            {
                OnOver((UIInteractiveGraphic)children[i]);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
        if (!down)
        {
            for (int i = 0; i < children.Length; i++)
            {
                OnOut((UIInteractiveGraphic)children[i]);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Deselect the previous game object that had the focus
        Scene.currentSelectedGameObject.gameObject.GetComponent<UIEvent>().OnGameObjectDeselect();

        //Assign the current game object as the current selected game object
        Scene.currentSelectedGameObject = gameObject.GetComponent<Selectable>().gameObject;
        down = true;

        for (int i = 0; i < children.Length; i++)
        {
            OnDown((UIInteractiveGraphic)children[i]);
        }

        onMouseUp = OnMouseUp();
        StartCoroutine(onMouseUp);
    }


    IEnumerator OnMouseUp()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StopCoroutine(onMouseUp);
                if (over)
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        OnUp((UIInteractiveGraphic)children[i]);
                    }

                }
                else
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        OnOutside((UIInteractiveGraphic)children[i]);
                    }
                }
            }
            yield return null;
        }
    }


    public void OnGameObjectSelect()
    {
        down = true;

        for (int i = 0; i < children.Length; i++)
        {
            OnSelect((UIInteractiveGraphic)children[i]);
        }
    }


    public void OnGameObjectDeselect()
    {
        down = false;

        for (int i = 0; i < children.Length; i++)
        {
            OnDeselect((UIInteractiveGraphic)children[i]);
        }
    }


    public abstract void OnOver(UIInteractiveGraphic child);

    public abstract void OnOut(UIInteractiveGraphic child);

    public abstract void OnDown(UIInteractiveGraphic child);

    public abstract void OnUp(UIInteractiveGraphic child);

    public abstract void OnOutside(UIInteractiveGraphic child);

    public abstract void OnSelect(UIInteractiveGraphic child);

    public abstract void OnDeselect(UIInteractiveGraphic child);
}
