using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public ItemData item;
    public int quantity;

    public Equipment EquipmentItem => item as Equipment;
    public ItemStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    
}
