using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
///  Behavior for when the ghost has initiated an attack and needs to go within range of the player for the animation to make sense
/// </summary>
public class AttackApproachBehavior : IEntityBehavior
{
    private Transform victimTransform;
    private Vector3 targetPosition;
    private Vector3 targetFacing;
    private float distance;
    private EntityController entity;

    public Action OnArriveAtTarget = () => { };
    private bool hasArrived;
    private float EPSILON = 0.001f;

    public AttackApproachBehavior(float distanceToStandAwayFromVictim, Transform victimTransform)
    {
        this.victimTransform = victimTransform;
        this.distance = distanceToStandAwayFromVictim;
        
    }

    public void Enter(EntityController entity)
    {
        entity.Freeze();
        this.targetPosition = getTargetPosition(entity.transform.position, victimTransform.position);
        this.targetFacing = victimTransform.position;
        this.targetFacing.y = entity.transform.position.y;
        this.entity = entity;
        tickLookAtPlayer();
    }

    public void Exit(EntityController entity)
    {
        Debug.Log("Exiting AttackApproachBehavior");
    }

    public void Tick(EntityController entity)
    {
        if(!hasArrived)
        {
            tickMoveToAttackPosition();
            if(checkHasArrived())
            {
                hasArrived = true;
                OnArriveAtTarget();
            }
        }

        tickLookAtPlayer();
    }

    private void tickLookAtPlayer()
    {
        targetFacing = victimTransform.position;
        targetFacing.y = entity.transform.position.y;
        entity.transform.LookAt(victimTransform);
    }

    private void tickMoveToAttackPosition()
    {
        // TODO
    }

    private Vector3 getTargetPosition(Vector3 ghostPos, Vector3 victimPos)
    {
        Vector3 directionFromPlayerToGhost = ghostPos - victimPos;
        directionFromPlayerToGhost.y = 0;
        Vector3 scaled = Vector3.Scale(Vector3.Normalize(directionFromPlayerToGhost),
            new Vector3(distance, 1, distance));

        return scaled + new Vector3(victimPos.x, ghostPos.y, victimPos.z);
    }

    private bool checkHasArrived()
    {
        return Vector3.Magnitude(entity.transform.position - targetPosition) < EPSILON;
    }
}
