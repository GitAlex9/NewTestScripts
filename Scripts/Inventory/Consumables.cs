using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Consumables")]
public class Consumables : ItemData
{
    public ConsumableType consumableType;
    public int recoveryPercent;
    public int speedBoost;
    public int damageBoost;
    ItemStack itemStack;
    
    public override void Use()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var entity = player.GetComponent<Entity>();
            if (entity != null)
            {
                switch (consumableType)
                {
                    case ConsumableType.Recovery:
                        float healAmount = entity.stats.MaxHealth * (recoveryPercent / 100);
                        entity.Heal(healAmount);
                        break;

                    case ConsumableType.SpeedBoost:
                        
                        //logic
                        break;

                    case ConsumableType.DamageBoost:

                        float damageIncrease = entity.stats.Strength * (damageBoost / 100f);
                        entity.PowerBoost(damageIncrease);
                        break;

                    
                }
            }
        }
    }

    
}
public enum ConsumableType
{
    Recovery,
    DamageBoost,
    DefenseBoost,
    SpeedBoost,
    AreaDamage,
    Antidote,
    
}
