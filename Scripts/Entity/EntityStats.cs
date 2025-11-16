using UnityEngine;

[System.Serializable]
public class EntityStats
{
    [SerializeField] private int level = 10;
    [SerializeField] private float health = 10;
    [SerializeField] private float maxHealth = 10;

    [SerializeField] private float strength = 0;
    [SerializeField] private float agility = 1;
    [SerializeField] private float defense = 1;

    public int Level { get => level; set => level = value; }
    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Strength { get => strength; set => strength = value; }
    public float Agility { get => agility; set => agility = value; }
    public float Defense { get => defense; set => defense = value; }

    public void SetStats(EntityStats newStats)
    {
        level = newStats.level;
        health = newStats.health;
        maxHealth = newStats.maxHealth;
        strength = newStats.strength;
        agility = newStats.agility;
        defense = newStats.defense;
    }

    // Soma os stats de outro EntityStats a este
    public void AddStats(EntityStats statsToAdd)
    {
        level += statsToAdd.level;
        health += statsToAdd.health;
        maxHealth += statsToAdd.maxHealth;
        strength += statsToAdd.strength;
        agility += statsToAdd.agility;
        defense += statsToAdd.defense;
    }

    // Retorna um novo EntityStats com a soma de dois EntityStats
    public static EntityStats operator +(EntityStats stats1, EntityStats stats2)
    {
        EntityStats result = new EntityStats();
        result.level = stats1.level + stats2.level;
        result.health = stats1.health + stats2.health;
        result.maxHealth = stats1.maxHealth + stats2.maxHealth;
        result.strength = stats1.strength + stats2.strength;
        result.agility = stats1.agility + stats2.agility;
        result.defense = stats1.defense + stats2.defense;
        return result;
    }

    // Cria uma cópia dos stats base e adiciona os bônus
    public EntityStats GetStatsWithBonus(EntityStats bonusStats)
    {
        EntityStats finalStats = new EntityStats();
        finalStats.SetStats(this);
        finalStats.AddStats(bonusStats);
        return finalStats;
    }
}
