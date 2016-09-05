
public class UIParent : UI
{
    public Groups group;
    public UI[] children;

    void Awake()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if(UIGroups.uiGroup[(int)group].elements != null)
            {
                UIGroups.uiGroup[(int)group].elements.Add((UIGraphic)children[i]);
            }
            
        }
    }
}
