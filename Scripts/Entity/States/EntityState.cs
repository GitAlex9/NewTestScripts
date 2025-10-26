using UnityEngine;

[CreateAssetMenu(menuName = "EntityState", fileName = "EntityState")]
public abstract class EntityState : ScriptableObject
{
    public virtual void Enter(Entity controller) { }
    public virtual void UpdateState(Entity controller) { }
    public virtual void Exit(Entity controller) { }
    public virtual void ReEnter(Entity controller) { }
}
