using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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

    public abstract void OnOver(UIInteractiveGraphic child);

    public abstract void OnOut(UIInteractiveGraphic child);

    public abstract void OnDown(UIInteractiveGraphic child);

    public abstract void OnUp(UIInteractiveGraphic child);

    public abstract void OnOutside(UIInteractiveGraphic child);
}
