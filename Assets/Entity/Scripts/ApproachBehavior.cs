using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
///  Behavior for when the ghost has initiated an attack and needs to go within range of the player for the animation to make sense,
///  or any other time the ghost needs to go in front of the player
/// </summary>
public class ApproachBehavior : IEntityBehavior
{
    private Transform victimTransform;
    private Vector3 targetPosition;
    private Vector3 targetFacing;
    private float distance;
    private EntityController entity;

    public Action OnArriveAtTarget = () => { };
    private bool hasArrived;
    private float EPSILON = 0.001f;

    public ApproachBehavior(float distanceToStandAwayFromVictim, Transform victimTransform)
    {
        this.victimTransform = victimTransform;
        this.distance = distanceToStandAwayFromVictim;
        
    }

    public void Enter(EntityController entity)
    {
        entity.Freeze();
        this.targetPosition = getTargetPosition(entity.transform.position, victimTransform.position);
        this.targetFacing = victimTransform.position;
        //this.targetFacing.y = entity.transform.position.y;
        this.targetFacing.y = 0;
        this.entity = entity;
        tickLookAtPlayer();
    }

    public void Exit(EntityController entity)
    {
        Debug.Log("Exiting AttackApproachBehavior");
        entity.Unfreeze();
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
        // getting rid of y doesnt work, probably because EntityController also has a look at player?
        // regardless it's not a big priority
        targetFacing = victimTransform.position;
        targetFacing.y = 0;
        entity.transform.LookAt(victimTransform);
    }

    private void tickMoveToAttackPosition()
    {
        entity.transform.position = Vector3.MoveTowards(entity.transform.position, targetPosition, 0.1f);
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
