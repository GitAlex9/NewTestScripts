using UnityEngine;
using System;

public class EntityMovement : MonoBehaviour
{
    [Header("Entity References")]
    [SerializeField] private EntityState walkState;
    [SerializeField] private EntityState dashState;

    [HideInInspector] public CharacterController characterController; 
    private Entity entity;

    [Header("Entity Movement")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f * 5f;
    public float turnSmoothTime = 0.1f;
    public float rotationSpeed = 5f;

    public Vector2 movementInput;
    public Vector2 moveDirection;
    public Vector3 moveDirectionForce;
    public float knockbackDistance = 0f;

    [HideInInspector] public float _moveSpeed = 0f;
    private float turnSmoothVelocity;
    [SerializeField] private float verticalVelocity;
    public bool isRunning = false;
    [SerializeField] private bool externalMovement = false;


    public float VerticalVelocity => verticalVelocity;

    [Header("Dash Settings")]
    public float dashDistance = 1.5f;
    public float dashSpeed = 10f;
    public float dashDistanceRan = 1.5f;
    public float dashCooldown = 1f;
    public float dashCooldownTimer = 0f;
    public Vector3 dashTargetPosition;
    public Vector3 dashDirection;

    [Header("Target Focus Settings")]
    public bool isFocusingTarget = false;
    public Transform currentFocusTarget;
    public float focusSearchRadius = 4f;
    public float focusSearchAngle = 160f;
    public float focusRotationSpeed = 10f;
    [Range(0f, 1f)]
    public float focusSpeedMultiplier = 0.7f; // 70% da velocidade normal
    public LayerMask targetLayerMask;

    // Movement Events
    public event Action<Entity> OnMovementStarted;
    public event Action<Entity> OnMovementStopped;

    public event Action<Entity> OnDashStarted;
    public event Action<Entity> OnDashStopped;

    public event Action<Entity> OnEntityFellOutOfBounds;
    
    public event Action<Transform> OnTargetFocused;
    public event Action OnTargetUnfocused;

    #region Unity Functions

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        entity = GetComponent<Entity>();
        _moveSpeed = moveSpeed;
    }

    private void Update()
    {
        // Cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Verificar validade do foco continuamente
        if (isFocusingTarget)
        {
            ValidateTargetFocus();
        }

        MovementPhysics();
    }

    #endregion

    #region Movement Physics

    public void MovementPhysics()
    {
        if (GameManager.Instance != null && (GameManager.Instance.SoftPaused || GameManager.Instance.IsPaused))
        {
            // Substitui o Update no script no playerEntity
            moveDirection = Vector2.zero;

            Vector3 stopMovement = new Vector3(0, VerticalForceCalculation(), 0);
            characterController.Move(stopMovement * Time.deltaTime);
            return;
        }

        Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y);

        // Aplicar multiplicador de velocidade se estiver focado
        float currentSpeed = _moveSpeed;
        if (isFocusingTarget)
        {
            currentSpeed *= focusSpeedMultiplier;
        }

        movement *= currentSpeed;
        movement.y = VerticalForceCalculation();

        if (movement.magnitude > 0.1f)
        {
            // Se não estiver focando, rotacionar normalmente na direção do movimento
            if (!isFocusingTarget && moveDirection.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            characterController.Move(movement * Time.deltaTime);

            if (characterController.transform.position.y < -50)
            {
                OnEntityFellOutOfBounds?.Invoke(entity);
            }
        }

    }
    
    public void ApplyJumpForce(float jumpForce)
    {
        // Fórmula física: v = sqrt(2 * g * h)
        // Onde v = velocidade inicial, g = gravidade, h = altura desejada
        verticalVelocity = Mathf.Sqrt(2f * gravity * jumpForce);
    }

    private float VerticalForceCalculation()
    {
        // Se tiver uma velocidade vertical positiva significativa, não resetar (permitir pulos)
        if (characterController.isGrounded && verticalVelocity <= 0)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }

    public void SetExternalMovement(bool value)
    {
        externalMovement = value;
        if (value)
        {
            moveDirection = Vector2.zero;
        }
    }

    public void OnMove(Vector2 direction)
    {
        movementInput = direction;
        if (externalMovement || !entity.entityIsAlive) return;

        // Se estiver focando um alvo, rotacionar para ele
        if (isFocusingTarget && currentFocusTarget != null)
        {
            UpdateTargetFocus();
        }

        moveDirection = movementInput;

        if (direction.magnitude > 0 && entity.currentState is StateGrounded && !(entity.currentState is StateWalk))
        {
            OnMovementStarted?.Invoke(entity);
            entity.TransitionToState(walkState);
        }
    }

    public void OnDash()
    {
        if (dashCooldownTimer > 0 || externalMovement) return;

        if (characterController.isGrounded && entity.currentState is not StateDash)
        {
            dashCooldownTimer = dashCooldown;
            OnDashStarted?.Invoke(entity);
            entity.TransitionToState(dashState);
        }
    }

    #endregion

    #region Target Focus

    public void OnFocusTargetPressed()
    {
        if (!isFocusingTarget)
        {
            // Procura e foca no inimigo mais próximo
            FocusNearestTarget();
        }
    }

    public void OnFocusTargetReleased()
    {
        if (isFocusingTarget)
        {
            // Desfoca ao soltar a tecla
            UnfocusTarget();
        }
    }

    private void FocusNearestTarget()
    {
        // Buscar todos os colliders em um raio ao redor da entidade
        Collider[] colliders = Physics.OverlapSphere(transform.position, focusSearchRadius);
        
        Transform nearestTarget = null;
        float bestWeightedDistance = float.MaxValue;
        Vector3 forwardDirection = transform.forward;

        foreach (Collider col in colliders)
        {
            // Verificar se é um inimigo (usando a tag configurada na entidade)
            if (col.CompareTag(entity.targetTag) && col.transform != transform)
            {
                Entity targetEntity = col.GetComponent<Entity>();
                // Verificar se a entidade está viva
                if (targetEntity != null && targetEntity.entityIsAlive)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    
                    // Calcular direção para o alvo (sem considerar Y)
                    Vector3 directionToTarget = (col.transform.position - transform.position);
                    directionToTarget.y = 0;
                    directionToTarget.Normalize();
                    
                    // Calcular alinhamento (-1 = atrás, 0 = lado, 1 = frente)
                    float alignment = Vector3.Dot(forwardDirection, directionToTarget);
                    
                    // Peso: inimigos à frente têm peso menor (são priorizados)
                    // alignment vai de -1 a 1, então (1 - alignment) vai de 0 a 2
                    // Multiplicar por 0.5 para ter um peso de 0 (frente) a 1 (atrás)
                    float directionWeight = (1f - alignment) * 0.5f;
                    
                    // Distância ajustada: inimigos à frente ficam "mais perto"
                    float weightedDistance = distance * (1f + directionWeight);
                    
                    if (weightedDistance < bestWeightedDistance)
                    {
                        bestWeightedDistance = weightedDistance;
                        nearestTarget = col.transform;
                    }
                }
            }
        }

        if (nearestTarget != null)
        {
            currentFocusTarget = nearestTarget;
            isFocusingTarget = true;
            OnTargetFocused?.Invoke(nearestTarget);
            Debug.Log($"[Target Focus] Focando em: {nearestTarget.name}");
        }
        else
        {
            Debug.Log($"[Target Focus] Nenhum alvo encontrado no raio de {focusSearchRadius}");
        }
    }

    private void UpdateTargetFocus()
    {
        if (currentFocusTarget == null)
        {
            UnfocusTarget();
            return;
        }

        // Verificar se o alvo ainda está vivo
        Entity targetEntity = currentFocusTarget.GetComponent<Entity>();
        if (targetEntity != null && !targetEntity.entityIsAlive)
        {
            UnfocusTarget();
            return;
        }

        // Verificar se o alvo ainda está no raio
        float distanceToTarget = Vector3.Distance(transform.position, currentFocusTarget.position);
        if (distanceToTarget > focusSearchRadius * 1.5f) // 50% extra de margem
        {
            UnfocusTarget();
            return;
        }

        // Rotacionar suavemente em direção ao alvo
        Vector3 directionToTarget = (currentFocusTarget.position - transform.position).normalized;
        directionToTarget.y = 0; // Manter apenas rotação horizontal

        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, focusRotationSpeed * Time.deltaTime);
        }
    }

    private void ValidateTargetFocus()
    {
        if (currentFocusTarget == null)
        {
            UnfocusTarget();
            return;
        }

        // Verificar se o alvo ainda está vivo
        Entity targetEntity = currentFocusTarget.GetComponent<Entity>();
        if (targetEntity != null && !targetEntity.entityIsAlive)
        {
            UnfocusTarget();
            return;
        }

        // Verificar se o alvo ainda está no raio
        float distanceToTarget = Vector3.Distance(transform.position, currentFocusTarget.position);
        if (distanceToTarget > focusSearchRadius * 1.5f) // 50% extra de margem
        {
            UnfocusTarget();
            return;
        }
    }

    public void UnfocusTarget()
    {
        if (isFocusingTarget)
        {
            isFocusingTarget = false;
            currentFocusTarget = null;
            OnTargetUnfocused?.Invoke();
        }
    }

    #endregion
}
