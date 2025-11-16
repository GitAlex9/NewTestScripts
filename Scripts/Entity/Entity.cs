using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Ink;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    public string targetTag = "Enemy";

    public BuffCountdown buffCountdown;
 
    [Header("State Machine")]
    public EntityState currentState;
    private List<EntityState> stateStack = new List<EntityState>();
    public bool blockStateTransitions = false;

    public EntityState idleState;
    public EntityState hitState;
    public EntityState deadState;

    [Header("Components")]
    public EntityMovement movementComponent;
    public EntityAction actionComponent;
    public EntityAnimationController animationController;

    // Stats
    [Header("Base Stats")]
    [SerializeField] public EntityStats baseStats = new EntityStats();
    [SerializeField] public EntityStats stats = new EntityStats();

    public bool blockAllStates = false;
    public bool entityIsAlive = true;

    [Header("Equipment")]
    public Equipment[] equipments = new Equipment[3];

    [Header("Damage Control")]
    public Damage lastDamageTaken;
    public event Action EntityHitted;
    public event Action<Entity> EntityDied;
    public event Action EntityStatsChanged;

    [Header("Som ao colidir")]
    public AudioSource source;
    public AudioClip[] hitEnemySFX;

    public void PlayHitSound()
    {
        AudioClip clip = hitEnemySFX[(int)UnityEngine.Random.Range(0, hitEnemySFX.Length)];
        if (clip == null) return;

        source.clip = clip;
        source.Play();
    }

    #region State Machine

    public void TransitionToState(EntityState state)
    {
        if (currentState == state || blockStateTransitions)
        {
            return;
        }
        RemoveState();
        currentState = state;
        currentState.Enter(this);
    }

    public void PopState()
    {
        currentState.Exit(this);

        if (stateStack.Count == 0)
        {
            ReturnDefaultState();
            return;
        }

        currentState = stateStack[stateStack.Count - 1];
        stateStack.RemoveAt(stateStack.Count - 1);

        currentState.ReEnter(this);
    }

    public void PopStateAfter(float time)
    {
        StartCoroutine(PopStateAfterCoroutine(time));
    }

    public void TransitionToStateAfter(EntityState state, float time)
    {
        StartCoroutine(TransitionToStateAfterCoroutine(state, time));
    }

    private System.Collections.IEnumerator TransitionToStateAfterCoroutine(EntityState state, float time)
    {
        yield return new WaitForSeconds(time);
        TransitionToState(state);
    }

    private System.Collections.IEnumerator PopStateAfterCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        PopState();
    }

    private void RemoveState()
    {
        if (currentState != null)
        {
            currentState.Exit(this);
            stateStack.Add(currentState);
            currentState = null;

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

    #region Unity Methods

    private void Awake()
    {
        movementComponent = GetComponent<EntityMovement>();
        actionComponent = GetComponent<EntityAction>();
        animationController = GetComponentInChildren<EntityAnimationController>();
    }

    public void Start()
    {
        EntityStart();
    }

    private void FixedUpdate()
    {
        if (CheckGameManagerBlock() || blockAllStates) return;

        currentState?.UpdateState(this);
    }

    #endregion

    #region Health and Hit

    public void TakeDamage(Damage damage)
    {
        if (!entityIsAlive) return;

        stats.Health -= damage.damageValue;
        lastDamageTaken = damage;

        PlayHitSound();
        TransitionToState(hitState);
        EntityHitted?.Invoke();
    }

    public void OnEntityDied()
    {
        EntityDied?.Invoke(this);
        TransitionToState(deadState);
        entityIsAlive = false;
    }

    public void Heal(float amount)
    {
        if (CompareTag("Player"))
        {
            stats.Health += amount;
            HealthEvents.TriggerCurrentHealthChanged(stats.Health);
        }
    }

    public void PowerBoost (float damageIncrease)
    {
        if (CompareTag("Player"))
        {
            
            buffCountdown.StartBuff(15f, () => { stats.Strength += damageIncrease; }, () => { RecalculateStats(); });
        }
    }

    public void UnblockStateControl(Entity entity)
    {
        entity.blockAllStates = false;
        entity.animationController.AnimationEnded -= entity.UnblockStateControl;
    }

    public void DestroySelf(Entity entity)
    {
        if (entity == null) return;

        // Animar escala para zero com efeito elástico
        entity.transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                Destroy(entity.gameObject);
            });
    }

    #endregion

    #region Equipment

    private void EquipStartingEquipment()
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            Equip(i);
        }
    }

    public void Equip(int slot)
    {   
        if (slot >= 0 && slot < equipments.Length && equipments[slot] != null)
        {
            equipments[slot].Equip(this, slot);        
            RecalculateStats();
        }
    }

    public void Unequip(int slot)
    {
        if (slot > 0 && slot < equipments.Length && equipments[slot] != null)
        {
            equipments[slot].Unequip(this, slot);
            RecalculateStats();
        }
    }

    public void RecalculateStats()
    {
        stats.SetStats(baseStats);

        if (Inventory.Instance != null)
        {
            foreach (var item in equipments)
            {
                if (item == null) continue;

                stats += item.stats;
            }
        }

        EntityStatsChanged?.Invoke();
    }

    #endregion

    public virtual void EntityStart()
    {
        EquipStartingEquipment();
        TransitionToState(idleState);

    }
}
