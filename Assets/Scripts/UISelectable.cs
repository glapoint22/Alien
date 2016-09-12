using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectable : UIInteractive {
    public Selectable selectableComponent;


    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (selectableComponent.interactable)
        {
            base.OnPointerEnter(eventData);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (selectableComponent.interactable)
        {
            base.OnPointerExit(eventData);
        }
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (selectableComponent.interactable)
        {
            //Deselect the previous game object that had the focus
            Scene.currentSelectedGameObject.gameObject.GetComponent<UIEvent>().OnGameObjectDeselect();

            //Assign the current game object as the current selected game object
            Scene.currentSelectedGameObject = gameObject.GetComponent<Selectable>().gameObject;

            base.OnPointerDown(eventData);
        }
    }
}
