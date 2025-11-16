using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateWalk", fileName = "StateWalk")]
public class StateWalk : EntityState
{
    public override void UpdateState(Entity entity)
    {
        TriggerMoveAnimation(entity);
        if (entity.movementComponent.moveDirection.magnitude < 0.1)
        {
            entity.TransitionToState(entity.idleState);
        }

    }

    public void TriggerMoveAnimation(Entity entity)
    {
        if (!entity.movementComponent.isRunning && entity.movementComponent.moveDirection.magnitude > 0.45f)
        {
            entity.animationController?.SetAnimationBool("IsRunning", true);
            entity.animationController?.SetAnimationBool("IsMoving", false);
            entity.movementComponent.isRunning = true;
        }
        else if (entity.movementComponent.moveDirection.magnitude <= 0.45f)
        {
            entity.animationController?.SetAnimationBool("IsMoving", true);
            entity.animationController?.SetAnimationBool("IsRunning", false);
            entity.movementComponent.isRunning = false;
        }    
    }


    public override void Enter(Entity entity)
    {
        base.Enter(entity);
        TriggerMoveAnimation(entity);

    }

    public override void Exit(Entity entity)
    {
        base.Exit(entity);
        entity.movementComponent.isRunning = false;
        entity.animationController?.SetAnimationBool("IsRunning", false);
        entity.animationController?.SetAnimationBool("IsMoving", false);
    }
}
