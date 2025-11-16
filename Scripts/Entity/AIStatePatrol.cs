using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AIState/StatePatrol", fileName = "AIStatePatrol")]
public class AIStatePatrol : EntityAIState
{
    public override void Enter(EntityAI brain)
    {
        NavMeshAgent agent = brain.agent;

        // Define o destino inicial para o agente
        Vector3 point;
        if (RandomPoint(brain.originalPosition, brain.transform.position, brain.patrolRadius, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }
    }

    public override void UpdateState(EntityAI brain)
    {
        NavMeshAgent agent = brain.agent;

        // Se tem caminho válido
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 directionToNext = agent.steeringTarget - brain.transform.position;
            directionToNext.y = 0; // Ignora altura para movimentos no plano XZ

            // Calcula direção para o controller de movimento
            Vector2 direction2D = new Vector2(directionToNext.normalized.x, directionToNext.normalized.z);
            brain.entity.movementComponent.OnMove(direction2D);
        }
        else
        {
            brain.entity.movementComponent.moveDirection = Vector2.zero;
            brain.TransitionToState(brain.idleState);
        }
        brain.DetectNearestTraget();
    }

    bool RandomPoint(Vector3 center, Vector3 entityPos, float range, out Vector3 result)
    {
        // Define o alcance mínimo (ajuste conforme necessário)
        float minRange = range * 0.4f; // 40% do alcance máximo como mínimo
        
        // Número máximo de tentativas para evitar loop infinito
        int maxAttempts = 30;
        int attempts = 0;
        
        while (attempts < maxAttempts)
        {
            attempts++;
            
            // Gera um vetor aleatório dentro da esfera
            Vector3 randomDirection = Random.insideUnitSphere;
            
            // Ajusta a distância para estar entre minRange e range
            float distance = Random.Range(minRange, range);
            Vector3 randomPoint = center + randomDirection.normalized * distance;
            
            // Verifica se o ponto está na NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                // Verifica se a distância real está acima do mínimo
                // (pode ser diferente após o SamplePosition)
                float actualDistance = Vector3.Distance(entityPos, hit.position);
                if (actualDistance >= minRange)
                {
                    result = hit.position;
                    return true;
                }
            }
        }
        
        // Se falhar após várias tentativas, aceita qualquer ponto válido
        Vector3 fallbackPoint = center + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(fallbackPoint, out NavMeshHit fallbackHit, range, NavMesh.AllAreas))
        {
            result = fallbackHit.position;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }
}
