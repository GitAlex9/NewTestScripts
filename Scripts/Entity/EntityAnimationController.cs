using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityAnimationController : MonoBehaviour
{
    [Header("References")]
    private Entity entity;
    private Animator animator;

    public bool AnimationIsPlaying => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    public int isWalkingHash;
    public int isRunnignHash;

    public event Action<Entity> AnimationStarted;
    public event Action<Entity> AnimationEnded;

    public void TriggerAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetAnimationBool(string animationName, bool value)
    {
        animator.SetBool(animationName, value);
    }

    public bool GetAnimationBool(string animationName, bool value)
    {
        return animator.GetBool(animationName);
    }

    public void OnAnimationEnded()
    {
        AnimationEnded?.Invoke(entity);
        Debug.Log("Animation Ended Event Triggered");
    }

    public void OnAttackAnimationHit()
    {
        
        entity.actionComponent?.OnAttackAnimationHit();
    }

    void Start()
    {
        entity = GetComponentInParent<Entity>();
        animator = GetComponentInChildren<Animator>();
    }
}
