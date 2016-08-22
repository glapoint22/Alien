using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : UIParent {
    [SerializeField]
    private Image progressImage;

    [SerializeField]
    private Text percent;

    public Text description;
    

    private float _progress;

    public float progress
    {
        set
        {
            _progress = value;
            progressImage.fillAmount = _progress;
            percent.text = (System.Math.Round(_progress, 2) * 100) + "%";
        }
    }


    
}
