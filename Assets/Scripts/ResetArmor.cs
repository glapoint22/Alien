using UnityEngine;
using UnityEngine.UI;

public class ResetArmor : MonoBehaviour
{
    [SerializeField]
    Button resetArmorButton;

    public void OnResetArmorButtonClick()
    {
        for (int i = 0; i < Characters.selectedCharacter.armor.Length; i++)
        {
            if (Characters.selectedCharacter.equipSlot[i].image.sprite != null)
            {
                Characters.selectedCharacter.armor[i] = (Armor)Characters.selectedCharacter.equipSlot[i].item;
            }
            else
            {
                Characters.selectedCharacter.armor[i] = Characters.selectedCharacter.race.baseArmor[i];
            }
        }

        Characters.selectedCharacter.UpdateArmor();

        resetArmorButton.interactable = false;
    }
}
