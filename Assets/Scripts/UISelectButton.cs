public class UISelectButton : UIEvent
{
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
}
