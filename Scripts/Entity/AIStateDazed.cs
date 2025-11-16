using UnityEngine;

[CreateAssetMenu(menuName = "AIState/StateDazed", fileName = "AIStateDazed")]
public class AIStateDazed : EntityAIState
{
    public override void Enter(EntityAI brain)
    {
        base.Enter(brain);
        brain.idleTimer = 1f;
    }

    public override void UpdateState(EntityAI brain)
    {
        brain.idleTimer -= Time.deltaTime;
        if (brain.idleTimer <= 0f)
        {
            brain.TransitionToState(brain.idleState);
        }
    }

}
