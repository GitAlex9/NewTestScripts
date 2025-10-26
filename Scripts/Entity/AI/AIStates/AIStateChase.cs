using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[CreateAssetMenu(menuName = "AIState/StateChase", fileName = "AIStateChase")]
public class AIStateChase : EntityAIState
{
    private float pathUpdateInterval = 0.2f; // Atualiza a cada 0.2 segundos
    private Coroutine pathUpdateCoroutine;

    public override void Enter(EntityAI brain)
    {
        if (brain.targetEntity != null)
        {
            NavMeshAgent agent = brain.agent;
            // Define o destino do agente como a posição do alvo
            agent.SetDestination(brain.targetEntity.transform.position);
            
            // Inicia a coroutine para atualizar o caminho periodicamente
        }
        else
        {
            Debug.LogWarning("Entity has no target to chase.");
            brain.TransitionToState(brain.idleState);
        }
    }

    private IEnumerator UpdatePathCoroutine(EntityAI brain)
    {
        while (brain.targetEntity != null)
        {
            // Atualiza o caminho periodicamente
            brain.agent.SetDestination(brain.targetEntity.transform.position);
            
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    public override void UpdateState(EntityAI brain)
    {
        NavMeshAgent agent = brain.agent;

        // Verifica se o alvo ainda existe
        if (brain.targetEntity == null)
        {
            brain.TransitionToState(brain.idleState);
            return;
        }
        
        brain.agent.SetDestination(brain.targetEntity.transform.position);

        // Usa distância ao quadrado (mais eficiente)
        float distanceToTarget = (brain.transform.position - brain.targetEntity.transform.position).magnitude;
        bool isTargetInRange = (brain.originalPosition - brain.targetEntity.transform.position).magnitude <= brain.chaseRadius;

        if (isTargetInRange)
        {
            if (agent.hasPath)
            {

                Vector3 directionToNext = agent.steeringTarget - brain.transform.position;
                directionToNext.y = 0; // Ignora altura para movimentos no plano XZ
                Vector2 direction2D = new Vector2(directionToNext.normalized.x, directionToNext.normalized.z);

                // Calcula direção para o controller de movimento
                if (distanceToTarget <= brain.attackRadius)
                {
                    if (brain.canAttack) brain.TransitionToState(brain.attackState);
                    else direction2D = Vector2.zero;
                }

                
                brain.entity.movementComponent.OnMove(direction2D);
            }
        }
        else
        {
            brain.TransitionToState(brain.idleState);
        }
        
    }

    public override void Exit(EntityAI brain)
    {
        base.Exit(brain);
        
        // Para a coroutine quando sai do estado
        if (pathUpdateCoroutine != null)
            brain.StopCoroutine(pathUpdateCoroutine);
            
        brain.entity.movementComponent.moveDirection = Vector2.zero;
    }
}
