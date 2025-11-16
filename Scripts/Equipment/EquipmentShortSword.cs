using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ShortSword", fileName = "ShortSword")]
public class EquipmentShortSword : Equipment
{
    private float hitDistance = 1.0f;
    private Vector3 hitBoxSize = new Vector3(0.5f, 0.5f, 1.0f);
    
    // Dictionary para rastrear entidades atingidas por cada atacante
    private Dictionary<Entity, HashSet<Entity>> hitEntitiesPerAttacker = new Dictionary<Entity, HashSet<Entity>>();

    private void SwordSlash(Entity entity)
    {
        // Garantir que existe uma entrada para esta entidade no dictionary
        if (!hitEntitiesPerAttacker.ContainsKey(entity))
        {
            hitEntitiesPerAttacker[entity] = new HashSet<Entity>();
        }
        
        // Limpar lista de entidades atingidas no início de cada ataque
        hitEntitiesPerAttacker[entity].Clear();
        
        entity.animationController?.TriggerAnimation($"Attack{entity.actionComponent.comboIndex + 1}");
        entity.actionComponent.attackAnimationInProgress = true;

        entity.actionComponent.comboStarted = true;
        entity.actionComponent.comboIndex++;
        entity.actionComponent.comboIndex = entity.actionComponent.comboIndex % 3;
    }

    private void SwordHit(Entity entity)
    {
        // Garantir que existe uma entrada para esta entidade
        if (!hitEntitiesPerAttacker.ContainsKey(entity))
        {
            hitEntitiesPerAttacker[entity] = new HashSet<Entity>();
        }
        
        // Calcula a posição na frente da entidade
        Vector3 boxCenter = entity.transform.position + entity.transform.forward * hitDistance;
        Vector3 boxHalfExtents = hitBoxSize * 0.5f;
        Quaternion boxOrientation = entity.transform.rotation;
        LayerMask hitMask = LayerMask.GetMask("Default"); // Ajuste para a layer dos alvos

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, boxOrientation, hitMask);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag(entity.targetTag) && hit.gameObject != entity.gameObject)
            {
                var targetEntity = hit.GetComponent<Entity>();
                if (targetEntity != null)
                {
                    // Verificar se já atingiu esta entidade neste ataque
                    if (!hitEntitiesPerAttacker[entity].Contains(targetEntity))
                    {
                        hitEntitiesPerAttacker[entity].Add(targetEntity);
                        Debug.Log($"Sword hit for {entity.stats.Strength} damage to {targetEntity.name}");
                        targetEntity.TakeDamage(new Damage(entity.stats.Strength, entity.gameObject, boxCenter));
                    }
                }
            }
        }
    }

    public void OnAttackAnimationEnded(Entity entity)
    {
        entity.actionComponent.attackAnimationInProgress = false;
        entity.actionComponent?.OnActionEnded();
    }

    public override void OnPressed(Entity entity)
    {
        SwordSlash(entity);
        entity.actionComponent?.OnActionStarted();
    }

    public override void Equip(Entity entity, int slot)
    {
        switch (slot)
        {
            case 0:
                entity.actionComponent.FirstActionPressed += OnPressed;
                break;
            case 1:
                entity.actionComponent.SecondActionPressed += OnPressed;
                break;
            case 2:
                entity.actionComponent.ThirdActionPressed += OnPressed;
                break;
        }
        
        entity.actionComponent.AttackHitted += SwordHit;
        entity.animationController.AnimationEnded += OnAttackAnimationEnded;
    }

    public override void Unequip(Entity entity, int slot)
    {
        switch (slot)
        {
            case 0:
                entity.actionComponent.FirstActionPressed -= OnPressed;
                break;
            case 1:
                entity.actionComponent.SecondActionPressed -= OnPressed;
                break;
            case 2:
                entity.actionComponent.ThirdActionPressed -= OnPressed;
                break;
        }
        
        entity.actionComponent.AttackHitted -= SwordHit;
        entity.animationController.AnimationEnded -= OnAttackAnimationEnded;
        
        // Limpar entrada do dictionary quando desequipar
        if (hitEntitiesPerAttacker.ContainsKey(entity))
        {
            hitEntitiesPerAttacker.Remove(entity);
        }
    }
}
