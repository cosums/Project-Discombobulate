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
        phaseTimer += Time.deltaTime;

        entity.NodeManager.EvaluateHiddenNodes(entity.Player);

        switch (phase)
        {
            case StalkingPhase.Hiding: TickHiding(entity); break;
            case StalkingPhase.Following: TickFollowing(entity); break;
            case StalkingPhase.StareDown: TickStareDown(entity); break;
        }
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
            entity.CurrentTarget = entity.NodeManager.FindFarthestSafeHiddenNode(entity.Player, entity.transform);
            entity.Agent.SetDestination(entity.CurrentTarget.transform.position);
            return;
        }
    }

    private void TickFollowing(EntityController entity)
    {
        if (entity.PlayerVisibility == Visibility.Focused && entity.HasLineOfSight)
        {
            SetPhase(StalkingPhase.StareDown);
            return;
        }
    }

    private void TickStareDown(EntityController entity)
    {
        if (phaseTimer == 0f)
        {
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
        
        // TODO
        entity.CurrentTarget = entity.NodeManager.FindFarthestSafeHiddenNode(entity.Player, entity.transform);
        entity.Agent.SetDestination(entity.CurrentTarget.transform.position);
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
