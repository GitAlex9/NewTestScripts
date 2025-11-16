using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateFall", fileName = "StateFall")]
public class StateFall : EntityState
{
    public override void UpdateState(Entity entity)
    {
        entity.movementComponent.MovementPhysics();

        if (entity.movementComponent.characterController.isGrounded)
        {
            entity.PopState();
        }
    }

    public override void ReEnter(Entity entity)
    {
        entity.PopState();
    }
}
