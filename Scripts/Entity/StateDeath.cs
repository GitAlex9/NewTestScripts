using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateDeath", fileName = "StateDeath")]
public class StateDeath : EntityState
{
    public override void Enter(Entity entity)
    {
        entity.blockStateTransitions = true;
        entity.animationController?.TriggerAnimation("Died");
        entity.animationController?.SetAnimationBool("OnDeath", true);

        entity.entityIsAlive = false;

        entity.animationController.AnimationEnded += OnDeathAnimationEnded;
    }

    private void OnDeathAnimationEnded(Entity entity)
    {
        if (entity.CompareTag("Player")) GameManager.Instance.PlayerDeath();
        else entity.DestroySelf(entity);

        entity.animationController.AnimationEnded -= OnDeathAnimationEnded;
    }
    
    public void OnPlayerDeath(Entity entity)
    {
        GameManager.Instance.PlayerDeath();
    }

    public override void Exit(Entity entity)
    {
        entity.blockStateTransitions = false;
    }

}
