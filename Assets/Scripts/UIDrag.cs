using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : UIInteractive, IBeginDragHandler, IDragHandler
{
    [SerializeField]
    private RectTransform parent;

    [SerializeField]
    private Texture2D dragCursor;


    private float xOffset;
    private float yOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        xOffset = parent.position.x - eventData.position.x;
        yOffset = parent.position.y - eventData.position.y;
    }

    public void OnDrag(PointerEventData data)
    {
        parent.position = new Vector2(data.position.x + xOffset, data.position.y + yOffset);
    }


    public override void OnOver(UIInteractiveGraphic child)
    {
        base.OnOver(child);
        Cursor.SetCursor(dragCursor, new Vector2(10, 10), CursorMode.Auto);
    }

    public override void OnOut(UIInteractiveGraphic child)
    {
        base.OnOut(child);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public override void OnOutside(UIInteractiveGraphic child)
    {
        base.OnOutside(child);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
