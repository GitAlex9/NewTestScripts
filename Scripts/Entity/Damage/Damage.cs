using UnityEngine;

public class Damage
{
    [Header("References")]
    public GameObject damageOwner;

    [Header("Damage Control")]
    public float damageValue = 10;
    public Vector3 damageOrigin;

    public Damage(float damageValue, GameObject owner, Vector3 dmgOrigin)
    {
        this.damageValue = damageValue;
        this.damageOwner = owner;
        this.damageOrigin = dmgOrigin;
    }
    
}
