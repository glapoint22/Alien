using UnityEngine;

public class UI : MonoBehaviour
{
    public static Color GetUIColor(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
