using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityAI : MonoBehaviour
{
    [Header("Entity References")]
    public CharacterController characterController;
    public Entity entity;

    [Header("State Machine")]
    [SerializeField] private EntityAIState _currentState;
    private List<EntityAIState> stateStack = new List<EntityAIState>();
    public bool blockStateTransition = false;

    [Header("AI States")]
    public EntityAIState idleState;
    public EntityAIState patrolState;
    public EntityAIState chaseState;
    public EntityAIState attackState;
    public EntityAIState fleeState;
    public EntityAIState deadState;
    public EntityAIState hitState;

    [Header("AI Settings")]
    public Vector3 originalPosition;
    public float patrolRadius = 5f;
    public float detectionRadius = 5f;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;
    public bool canFlee = false;
    public bool agressive = false;

    [Header("Flags")]
    public bool canAttack = true;

    [Header("Timers")]
    public float idleTimerMax = 2f;
    public float attackCooldownMax = 1f;

    public float attackCooldown = 0f;
    public float idleTimer = 0f;

    public Entity targetEntity;

    [Header("References")]
    public NavMeshAgent agent;

    #region State Machine

    public void TransitionToState(EntityAIState state)
    {
        if (_currentState == state || blockStateTransition)
        {
            return;
        }

        RemoveState();
        _currentState = state;
        _currentState.Enter(this);
    }

    public void PopState()
    {
        _currentState.Exit(this);

        if (stateStack.Count == 0)
        {
            ReturnDefaultState();
            return;
        }

        _currentState = stateStack[stateStack.Count - 1];
        stateStack.RemoveAt(stateStack.Count - 1);

        _currentState.ReEnter(this);
    }

    private void RemoveState()
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
            stateStack.Add(_currentState);
            _currentState = null;

            if (stateStack.Count > 5)
            {
                stateStack.RemoveAt(0);
            }
        }
    }

    public virtual void ReturnDefaultState()
    {
        TransitionToState(idleState);
    }

    private bool CheckGameManagerBlock()
    {
        return GameManager.Instance != null && (GameManager.Instance.SoftPaused || GameManager.Instance.IsPaused);
    }

    #endregion

    private void Start()
    {
        entity = GetComponent<Entity>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.speed = entity.movementComponent.moveSpeed;
        }

        originalPosition = transform.position;
        entity.EntityDied += OnEntityDeath;
        entity.EntityHitted += OnEntityHit;

        // Initialize the brain with the idle state
        TransitionToState(idleState);
    }

    private void OnDisable()
    {
        entity.EntityDied -= OnEntityDeath;
        entity.EntityHitted -= OnEntityHit;
    }

    private void FixedUpdate()
    {
        _currentState?.UpdateState(this);
        RunCooldownTimers();
    }

    public void DetectNearestTraget()
    {
        // Busca colliders na área de detecção
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        Entity nearestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            // Verifica se o collider tem uma Entity e se é um inimigo
            Entity potentialTarget = hitCollider.GetComponent<Entity>();

            if (potentialTarget != null && potentialTarget.CompareTag(entity.targetTag))
            {
                // Calcula distância
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                // Verifica se é o inimigo mais próximo
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = potentialTarget;
                }
            }
        }
        if (nearestEnemy != null)
        {
            targetEntity = nearestEnemy;
            float distanceToTarget = Vector3.Distance(transform.position, targetEntity.transform.position);
            if (distanceToTarget <= attackRadius && canAttack)
            {
                TransitionToState(attackState);
            }
            else
            {
                TransitionToState(chaseState);
            }

        }
    }

    public void RunCooldownTimers()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0f)
            {
                canAttack = true;
                attackCooldown = attackCooldownMax;
            }
        }
    }

    public void ReturnToIdle(Entity entity)
    {
        targetEntity = null;
        TransitionToState(idleState);
    }

    public void OnEntityDeath(Entity entity)
    {
        targetEntity = null;
        TransitionToState(deadState);
    }
    
    public void OnEntityHit()
    {
        if (entity.entityIsAlive)
        {
            TransitionToState(hitState);
        }
    }
}
