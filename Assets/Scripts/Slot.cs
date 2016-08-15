using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    //Public
    public RectTransform rectTransform;
    public Text itemName;
    public Image image;
    public Button button;

    //Private
    private int stackCnt;
    private Item Item;


    public int stackCount
    {
        get
        {
            return stackCnt;
        }
        set
        {
            stackCnt = value;

            if (item.isStackable)
            {
                itemName.text = item.name + " (" + stackCnt + ")";
            }
            else
            {
                itemName.text = item.name;
            }
        }
    }


    public Item item
    {
        get
        {
            return Item;
        }
        set
        {
            Item = value;

            if(Item != null) image.sprite = Item.image;

        }
    }
}
