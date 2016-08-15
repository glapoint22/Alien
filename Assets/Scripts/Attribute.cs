using UnityEngine;

public struct Attribute
{
    public int value;
    public int baseValue;
    public float levelExponent;

    public void CalculateValue(int level)
    {
        value = (int)Mathf.Pow(level, levelExponent) + baseValue - 1;
    }
}
