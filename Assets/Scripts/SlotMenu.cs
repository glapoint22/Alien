using UnityEngine;

public class SlotMenu : MonoBehaviour
{
    public GameObject[] menuButton;
    public ItemStack itemStack;

    public void ResetMenuButtons()
    {
        //Reset all the buttons
        for (int i = 0; i < menuButton.Length; i++)
        {
            menuButton[i].SetActive(false);
        }
    }
}
