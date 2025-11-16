using UnityEngine;

[CreateAssetMenu(menuName = "AIState/StateIdle", fileName = "AIStateIdle")]
public class AIStateIdle : EntityAIState
{
    public override void Enter(EntityAI brain)
    {
        base.Enter(brain);
        brain.idleTimer = brain.idleTimerMax;
    }

    public override void UpdateState(EntityAI brain)
    {
        brain.idleTimer -= Time.deltaTime;
        if (brain.idleTimer <= 0f)
        {
            brain.TransitionToState(brain.patrolState);
        }

        brain.DetectNearestTraget();
    }
    
}
