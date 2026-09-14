using UnityEngine;

public class StalkingBehavior : IEntityBehavior
{
    private StalkingPhase phase;
    private float phaseTimer;
    
    public void Enter(EntityController entity)
    {
        phase = StalkingPhase.Following;
        phaseTimer = 0f;
    }

    public void Tick(EntityController entity)
    {
        entity.NodeManager.EvaluateHiddenNodes(entity.Player);

        switch (phase)
        {
            case StalkingPhase.Hiding: TickHiding(entity); break;
            case StalkingPhase.Following: TickFollowing(entity); break;
            case StalkingPhase.StareDown: TickStareDown(entity); break;
        }
        
        phaseTimer += Time.deltaTime;
    }

    private void SetPhase(StalkingPhase p)
    {
        phase = p;
        phaseTimer = 0f;
    }

    private void TickHiding(EntityController entity)
    {
        // check if offscreen
        if (entity.PlayerVisibility == Visibility.Offscreen || !entity.HasLineOfSight)
        {
            entity.CurrentTarget = entity.NodeManager.FindRandHiddenNode(entity.Player);
            entity.Agent.Warp(entity.CurrentTarget.transform.position); // go to a new place. will probably need a cooldown here too
            entity.Agent.speed = entity.StalkFollowSpeed;
            SetPhase(StalkingPhase.Following); // move to following
            return;
        }

        // check if hiding node is now visible
        if (entity.CurrentTarget.VisibleToPlayer && entity.CurrentTarget.LineOfSightToPlayer)
        {
            AINode target = entity.NodeManager.FindClosestSafeHiddenNode(entity.Player, entity.transform);
            entity.MoveToNode(target); // do not be concerened with staying out of sight, just GO!
            return;
        }
    }

    private void TickFollowing(EntityController entity)
    {
        if ((entity.PlayerVisibility == Visibility.Focused || entity.PlayerVisibility == Visibility.Periphery) && entity.HasLineOfSight)
        {
            SetPhase(StalkingPhase.StareDown);
            return;
        }

        AINode targetNode = entity.NodeManager.FindClosestOffscreenNode(entity.Player, entity.StalkTargetDistance);
        if (targetNode != entity.CurrentTarget) {
            entity.SmartMoveToNode(targetNode, entity.NodeManager.PathExposureCostFunc); // try to stay out of sight!
        }
    }

    private void TickStareDown(EntityController entity)
    {
        if (phaseTimer == 0f)
        {
            // TODO
            float f = Random.Range(0, 1f);
            if (f < .25f)
            {
                // do a staredown and then flicker lights to vanish
            } 
            else
            {
                // retreat out of sight
            }
        }
        
        
        AINode target = entity.NodeManager.FindClosestSafeHiddenNode(entity.Player, entity.transform);
        entity.MoveToNode(target);


        entity.Agent.speed = entity.StalkHideSpeed;
        SetPhase(StalkingPhase.Hiding);
    }

    public void Exit(EntityController entity)
    {
        //throw new System.NotImplementedException();
    }

    public enum StalkingPhase
    {
        Hiding,
        Following,
        StareDown
    }
}
