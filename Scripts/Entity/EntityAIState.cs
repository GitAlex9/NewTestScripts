using UnityEngine;

[CreateAssetMenu(menuName = "AI/EntityAIState", fileName = "EntityAIState")]
public class EntityAIState : ScriptableObject
{
    public virtual void Enter(EntityAI controller) { }
    public virtual void UpdateState(EntityAI controller) { }
    public virtual void Exit(EntityAI controller) { }
    public virtual void ReEnter(EntityAI controller) { }
}
