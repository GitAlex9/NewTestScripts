using UnityEngine;
using System;

public class EntityAction : MonoBehaviour
{
    [Header("Entity References")]
    public Entity entity;
    [SerializeField] private EntityState attackState;

    [Header("Entity Action")]
    [SerializeField] private int actionBufferFramesMax = 5;
    private int actionBufferFrames = 0;
    private bool actionBuffer = false;

    public bool actionInProgress = false;
    public Transform attackPosition;

    [Header("Combo Attack")]
    public float attackComboLeeway = 1f;
    public GameObject slashDamageBox;
    public int comboIndex = 0;
    public bool comboStarted = false;
    public bool attackAnimationInProgress = false;

    [Header("Step Settings")]
    public float stepDistance = 0.5f;
    public float stepSpeed = 5f;
    public float stepDistanceRan = 0;

    public Vector3 startPosition;
    public Vector3 targetPosition;
    public Vector3 stepDirection;

    // Events
    public event Action<Entity> FirstActionPressed;
    public event Action<Entity> FirstActionReleased;

    public event Action<Entity> SecondActionPressed;
    public event Action<Entity> SecondActionReleased;

    public event Action<Entity> ThirdActionPressed;
    public event Action<Entity> ThirdActionReleased;

    public event Action<Entity> ActionStarted;
    public event Action<Entity> ActionEnded;
    public event Action<Entity> AttackHitted;

    #region Action Input

    private void actionControl()
    {
        if (actionBuffer)
        {
            actionBufferFrames++;
            if (actionBufferFrames >= actionBufferFramesMax)
            {
                actionBuffer = false;
                actionBufferFrames = 0;
            }
        }
    }

    public void OnActionStarted()
    {
        actionInProgress = true;
        entity.TransitionToState(attackState);
        ActionStarted?.Invoke(entity);
    }

    public void OnActionEnded()
    {
        actionInProgress = false;
        ActionEnded?.Invoke(entity);
        entity.PopState();

        if (actionBuffer && comboIndex > 0)
        {
            OnFirstActionPressed();
        }
        else
        {
            comboIndex = 0;
            comboStarted = false;
        }
        actionBuffer = false;
    }

    public void OnFirstActionPressed()
    {
        if (attackAnimationInProgress)
        {
            actionBuffer = true;
            return;
        }

        FirstActionPressed?.Invoke(entity);
    }

    public void OnFirstActionReleased()
    {
        FirstActionReleased?.Invoke(entity);
    }

    public void OnSecondActionPressed()
    {
        SecondActionPressed?.Invoke(entity);
    }

    public void OnSecondActionReleased()
    {
        SecondActionReleased?.Invoke(entity);
    }

    public void OnThirdActionPressed()
    {
        ThirdActionPressed?.Invoke(entity);
    }

    public void OnThirdActionReleased()
    {
        ThirdActionReleased?.Invoke(entity);
    }
    
    #endregion

    public void OnAttackAnimationHit()
    {
        AttackHitted?.Invoke(entity);
    }

    private void FixedUpdate()
    {
        actionControl();
    }

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }
}
