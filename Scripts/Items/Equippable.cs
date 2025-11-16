using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/EquippableItem")]
public class Equippable : ItemData
{
    public EquipmentSlot equipSlot;
    public int attack;
    public int defense;
    public int baseAttack; 
    public int baseDefense;
    public int critChance;
    public int critDamage;
    public int HP;
    


    
    public override void Use()
    {
        RemoveFromInventory();        
    }
}
public enum EquipmentSlot

{
    Weapon,
    Armor,
    Helmet,
    Boots,
    Shield,
    Special

}
