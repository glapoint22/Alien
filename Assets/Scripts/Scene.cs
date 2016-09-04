using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scene : MonoBehaviour {
    public EventSystem system;
    private Selectable next = null;

    public void SetFocus(GameObject focusGameObject)
    {
        system = EventSystem.current;
        system.SetSelectedGameObject(focusGameObject, new BaseEventData(system));
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            else
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }
}
