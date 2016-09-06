using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIButton : UIEvent
{
    //private bool _isEnabled;

    //public bool isEnabled
    //{
    //    get
    //    {
    //        return _isEnabled;
    //    }
    //    set
    //    {
    //        _isEnabled = value;
    //        if (_isEnabled)
    //        {
    //            gameObject.transform.GetComponent<Button>().interactable = true;

    //            for (int i = 0; i < children.Length; i++)
    //            {
    //                OnOut((UIInteractiveGraphic)children[i]);
    //            }
    //        }
    //        else
    //        {
    //            gameObject.transform.GetComponent<Button>().interactable = false;

    //            for (int i = 0; i < children.Length; i++)
    //            {
    //                OnDisabled((UIInteractiveGraphic)children[i]);
    //            }
    //        }

    //    }
    //}
    [SerializeField]
    private Button button;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            base.OnPointerEnter(eventData);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            base.OnPointerExit(eventData);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
        {
            base.OnPointerDown(eventData);
        }
    }



    public override void OnOver(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.overAlpha);
    }

    public override void OnOut(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.alpha);
    }

    public override void OnDown(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.downAlpha);
    }

    public override void OnUp(UIInteractiveGraphic child)
    {
        down = false;
        child.graphic.color = GetUIColor(child.graphic.color, child.upAlpha);
    }

    public override void OnOutside(UIInteractiveGraphic child)
    {
        down = false;
        child.graphic.color = GetUIColor(child.graphic.color, child.alpha);
    }

    public override void OnSelect(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.selectAlpha);
    }

    public override void OnDeselect(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.alpha);
    }

    private void OnDisabled(UIInteractiveGraphic child)
    {
        child.graphic.color = GetUIColor(child.graphic.color, child.disabledAlpha);
    }
}
