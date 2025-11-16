using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateGrounded", fileName = "StateGrounded")]
public class StateGrounded : EntityState
{
    public override void Enter(Entity entity)
    {
        //entity.movementComponent.verticalVelocity = -1f;

    }

    public override void Exit(Entity entity)
    {
    }

    public override void ReEnter(Entity entity)
    {
        Enter(entity);
    }
}
