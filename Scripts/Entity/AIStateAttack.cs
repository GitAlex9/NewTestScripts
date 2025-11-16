using UnityEngine;

[CreateAssetMenu(menuName = "AIState/StateAttack", fileName = "AIStateAttack")]
public class AIStateAttack : EntityAIState
{
    public override void Enter(EntityAI aiController)
    {
        base.Enter(aiController);
        aiController.canAttack = false;
        aiController.attackCooldown = aiController.attackCooldownMax;

        aiController.entity.actionComponent.OnFirstActionPressed(); 
        aiController.entity.actionComponent.OnFirstActionReleased();      
    }

    public override void UpdateState(EntityAI aiController)
    {
        if (!aiController.entity.actionComponent.actionInProgress)
        {
            aiController.TransitionToState(aiController.idleState);
        }
    }
}
