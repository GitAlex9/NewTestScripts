using UnityEngine;

[CreateAssetMenu(menuName = "AIState/StateDead", fileName = "AIStateDead")]
public class AIStateDead : EntityAIState
{
    public override void Enter(EntityAI brain)
    {
        base.Enter(brain);
        brain.blockStateTransition = true;
    }
}
