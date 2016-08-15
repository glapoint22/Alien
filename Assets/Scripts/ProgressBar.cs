using UnityEngine.UI;

public class ProgressBar : UIParent {
    private float _progress;

    public float progress
    {
        set
        {
            _progress = value;

            for(int i = 0; i < children.Length - 1; i++)
            {
                UIGraphic child = (UIGraphic)children[i];
                Image image = (Image)child.graphic;
                image.fillAmount = _progress;
            }
        }
    }
}
