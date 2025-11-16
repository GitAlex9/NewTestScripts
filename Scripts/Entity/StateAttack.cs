using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateAttack", fileName = "StateAttack")]
public class StateAttack : EntityState
{
    public override void Enter(Entity entity)
    {
        entity.actionComponent.ActionEnded += Exit;
    }

    private void OnActionEnded(Entity entity)
    {
        entity.actionComponent.ActionEnded -= OnActionEnded;
        entity.PopState();
    }
}
