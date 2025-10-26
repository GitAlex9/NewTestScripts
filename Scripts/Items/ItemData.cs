using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;    
    public string itemDescription;
    public Sprite icon;
    public int maxStack;
    public bool isStackable;
    public int upgradeLevel;
    public int maxUpgradeLevel;

    [Header("Upgrade Path for Equipment")]
    public Equipment nextUpgrade;

    public virtual void Use()
    {
        //What happens when using the item, rec hp, boost atk, etc

        Debug.Log("Using" + itemName);

    }
    
    public void RemoveFromInventory()
    {
        Inventory.Instance.RemoveItem(this);
    }

    //public void RemoveFromEquipment()
    //{
       // Inventory.Instance.RemoveItem(this);
   // }
    
}
