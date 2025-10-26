using UnityEngine;

public class DamageTester : MonoBehaviour
{
    [SerializeField] private Entity entity; // Drag your player Entity here in Inspector
    [SerializeField] private int testDamage = 10;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            var dmg = new Damage(testDamage, gameObject, transform.position);
            entity.TakeDamage(dmg);
        }
    }
}
