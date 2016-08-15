using System;
using UnityEngine;

public class InventorySlot : Slot
{
    [NonSerialized]
    public int[] upgradesPurchased;

    [NonSerialized]
    public int category;

    [NonSerialized]
    public int slotNum;

    [NonSerialized]
    public EquipMenuButton equipMenuButton;
}
