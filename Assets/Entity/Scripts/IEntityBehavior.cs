using UnityEngine;

public interface IEntityBehavior
{
    void Enter(EntityController entity);
    void Tick(EntityController entity);
    void Exit(EntityController entity);
}
