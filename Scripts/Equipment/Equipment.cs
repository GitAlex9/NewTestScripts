using System;
using Ink;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Equipment", fileName = "Equipment/Equipment")]
public class Equipment : ItemData
{
    public string Name = "Equipment";
    public EntityStats stats;
    
    public virtual void Equip(Entity entity, int slot) { }
    public virtual void Unequip(Entity entity, int slot) { }
    public virtual void EquipmentUpdate(Entity entity) { }

    public virtual void OnPressed(Entity entity) { }
    public virtual void OnReleased(Entity entity) { }
}
