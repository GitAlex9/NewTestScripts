using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateActAndStep", fileName = "StateActAndStep")]
public class StateActAndStep : EntityState
{
    [Header("Target Assistance")]
    [SerializeField] private bool enableTargetAssist = true;
    [SerializeField] private float targetSearchRadius = 5f;
    [SerializeField] private float targetAssistAngle = 45f; // Ângulo máximo para ajustar mira
    
    public override void Enter(Entity entity)
    {
        base.Enter(entity);

        Vector2 direction = entity.movementComponent.movementInput;
        Vector3 attackDirection;
        
        // Se estiver focando um alvo, usar a direção do foco
        if (entity.movementComponent.isFocusingTarget && entity.movementComponent.currentFocusTarget != null)
        {
            Vector3 directionToTarget = (entity.movementComponent.currentFocusTarget.position - entity.transform.position).normalized;
            directionToTarget.y = 0; // Manter no plano horizontal
            attackDirection = directionToTarget;
        }
        else if (direction.magnitude > 0)
        {
            attackDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }
        else
        {
            attackDirection = entity.transform.forward;
        }

        // Tentar encontrar inimigo próximo e ajustar direção (apenas se NÃO estiver focando)
        if (enableTargetAssist && !entity.movementComponent.isFocusingTarget)
        {
            Entity nearestEnemy = FindNearestEnemy(entity, attackDirection);
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = (nearestEnemy.transform.position - entity.transform.position).normalized;
                directionToEnemy.y = 0; // Manter no plano horizontal
                
                // Calcular ângulo entre direção atual e direção do inimigo
                float angleToEnemy = Vector3.Angle(attackDirection, directionToEnemy);
                
                // Se o inimigo estiver dentro do ângulo de assistência, ajustar mira
                if (angleToEnemy <= targetAssistAngle)
                {
                    attackDirection = directionToEnemy;
                }
            }
        }

        entity.actionComponent.stepDirection = attackDirection;

        // Rotaciona a entidade para a direção do ataque imediatamente
        if (entity.actionComponent.stepDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(entity.actionComponent.stepDirection);
            entity.transform.rotation = targetRotation;
        }

        entity.actionComponent.stepDistanceRan = entity.actionComponent.stepDistance;

        // Para o movimento normal do controller durante a ação
        entity.movementComponent.SetExternalMovement(true);

        entity.movementComponent._moveSpeed = entity.actionComponent.stepSpeed;
    }
    
    private Entity FindNearestEnemy(Entity attackerEntity, Vector3 attackDirection)
    {
        Collider[] hitColliders = Physics.OverlapSphere(attackerEntity.transform.position, targetSearchRadius);
        
        Entity nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider collider in hitColliders)
        {
            Entity potentialEnemy = collider.GetComponent<Entity>();
            
            // Verificar se é uma entidade válida e é um inimigo
            if (potentialEnemy != null && 
                potentialEnemy != attackerEntity && 
                potentialEnemy.CompareTag(attackerEntity.targetTag))
            {
                float distance = Vector3.Distance(attackerEntity.transform.position, potentialEnemy.transform.position);
                
                // Preferir inimigos que estão mais alinhados com a direção do ataque
                Vector3 directionToEnemy = (potentialEnemy.transform.position - attackerEntity.transform.position).normalized;
                float alignment = Vector3.Dot(attackDirection, directionToEnemy);
                
                // Usar distância ajustada por alinhamento (inimigos na frente têm prioridade)
                float weightedDistance = distance * (2f - alignment); // alignment varia de -1 a 1, então (2-alignment) varia de 1 a 3
                
                if (weightedDistance < nearestDistance)
                {
                    nearestDistance = weightedDistance;
                    nearestEnemy = potentialEnemy;
                }
            }
        }
        
        return nearestEnemy;
    }

    public override void UpdateState(Entity entity)
    {
        base.UpdateState(entity);
        // Verifica se há bloqueio no caminho usando Raycast
        Vector3 origin = entity.transform.position;
        Vector3 direction = entity.actionComponent.stepDirection.normalized;
        float distance = entity.actionComponent.stepDistance;
        bool blocked = Physics.Raycast(origin, direction, distance);

        if (entity.actionComponent.stepDistanceRan > 0f)
        {
            entity.movementComponent.moveDirection = new Vector2(entity.actionComponent.stepDirection.x, entity.actionComponent.stepDirection.z);
        }
        else
        {
            entity.movementComponent.moveDirection = Vector2.zero;
        }
        entity.actionComponent.stepDistanceRan -= entity.movementComponent._moveSpeed * Time.deltaTime;

    }

    public override void Exit(Entity entity)
    {
        base.Exit(entity);

        entity.movementComponent.moveDirection = Vector2.zero;
        entity.movementComponent._moveSpeed = entity.movementComponent.moveSpeed;
        // Restaura o controle normal de movimento
        entity.movementComponent.SetExternalMovement(false);
    }

    public override void ReEnter(Entity entity)
    {
        entity.PopState();
    }
}