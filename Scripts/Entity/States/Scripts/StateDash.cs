using UnityEngine;

[CreateAssetMenu(menuName = "EntityState/StateDash", fileName = "StateDash")]
public class StateDash : EntityState
{
    public override void Enter(Entity entity)
    {
        base.Enter(entity);

        // Determinar qual animação de dash usar
        string dashAnimation = GetDashAnimation(entity);
        entity.animationController?.TriggerAnimation(dashAnimation);

        entity.movementComponent.dashDistanceRan = entity.movementComponent.dashDistance;

        // Usa a direção do movimento
        Vector3 moveDir = new Vector3(entity.movementComponent.moveDirection.x, 0, entity.movementComponent.moveDirection.y);

        // Calcula a posição alvo
        entity.movementComponent.dashTargetPosition = entity.transform.position + (moveDir * entity.movementComponent.dashDistance);

        // Para o movimento normal do controller durante a ação
        entity.movementComponent.SetExternalMovement(true);
    }

    private string GetDashAnimation(Entity entity)
    {
        // Se não estiver focando, sempre usa DashFront
        if (!entity.movementComponent.isFocusingTarget || entity.movementComponent.currentFocusTarget == null)
        {
            return "DashFront";
        }

        // Quando está focando, determinar direção relativa à rotação do personagem
        Vector2 input = entity.movementComponent.movementInput;
        
        // Se não há input, usar DashBack (dash para trás)
        if (input.magnitude < 0.1f)
        {
            return "DashBack";
        }

        // Normalizar input
        input.Normalize();

        // Converter input 2D para 3D no mundo
        Vector3 inputWorld = new Vector3(input.x, 0, input.y);
        
        // Transformar o input para o espaço local do personagem
        // Isso considera a rotação atual do personagem
        Vector3 inputLocal = entity.transform.InverseTransformDirection(inputWorld);
        
        // Calcular ângulo baseado na direção local
        float angle = Mathf.Atan2(inputLocal.x, inputLocal.z) * Mathf.Rad2Deg;
        
        // Determinar animação baseada no ângulo local
        // -45 a 45 = Frente (em relação à onde está olhando)
        // 45 a 135 = Direita
        // 135 a -135 = Trás
        // -135 a -45 = Esquerda
        
        if (angle >= -45f && angle < 45f)
        {
            return "DashFront";
        }
        else if (angle >= 45f && angle < 135f)
        {
            return "DashRight";
        }
        else if (angle >= 135f || angle < -135f)
        {
            return "DashBack";
        }
        else // angle >= -135f && angle < -45f
        {
            return "DashLeft";
        }
    }

    public override void UpdateState(Entity entity)
    {
        base.UpdateState(entity);

        EntityMovement movementComponent = entity.movementComponent;

        // Move a entidade em direção ao alvo
        Vector3 currentPos = movementComponent.transform.position;
        Vector3 direction = (movementComponent.dashTargetPosition - currentPos).normalized;

        // Calcula a distância restante

        if (movementComponent.dashDistanceRan > 0f)
        {
            // Move usando o CharacterController
            Vector3 movement = direction * movementComponent.dashSpeed * Time.deltaTime;
            movement.y = movementComponent.VerticalVelocity * Time.deltaTime;
            movementComponent.dashDistanceRan -= movement.magnitude;
            movementComponent.characterController.Move(movement);

            // Rotaciona a entidade para a direção do movimento (se habilitado)
            if (direction.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.LerpAngle(movementComponent.transform.eulerAngles.y, targetAngle, Time.deltaTime * movementComponent.rotationSpeed);
                movementComponent.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        } else
        {
            // Ao chegar no destino, volta para o estado de idle ou outro apropriado
            entity.PopState();
        }
               
    }

    public override void Exit(Entity entity)
    {
        base.Exit(entity);

        // Restaura o controle normal de movimento
        entity.movementComponent.SetExternalMovement(false);
        entity.movementComponent.moveDirection = Vector2.zero;
    }
}