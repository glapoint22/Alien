using UnityEngine;
using UnityEngine.UI;

public class ItemStack : MonoBehaviour
{
    public InputField stackCountInputField;

    [System.NonSerialized]
    public int stackCount;

    [System.NonSerialized]
    public int stackCap;

    
    public void SetStackCount(int value)
    {
        //Increment or decrement the stack count
        stackCount += value;
        
        //Assign the value to the text
        stackCountInputField.text = stackCount.ToString();
    }

    public void OnValueChange()
    {
        //If string is empty, set entry as 1
        if(stackCountInputField.text == string.Empty)
        {
            stackCountInputField.text = "1";
            stackCount = 1;
            return;
        }

        //Get the stack count
        stackCount = int.Parse(stackCountInputField.text);

        //If stack count is less than 1
        if (stackCount < 1)
        {
            stackCountInputField.text = "1";
            stackCount = 1;
            return;
        }
            
        //If stack count is greater than the stack cap
        if (stackCap > 0)
        {
            if (stackCount > stackCap)
            {
                stackCountInputField.text = stackCap.ToString();
                stackCount = stackCap;
            }
        }
    }
}
