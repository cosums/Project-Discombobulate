using UnityEngine;

public class StalkingBehavior : IEntityBehavior
{
    private const float VanishChance = 0.25f;
    private const float StareDuration = 2f;
    private const float FlickerDuration = 1.2f;
    private const float MaxStareTime = 6f;
    private const float MinHideTime = 3f;
    
    private StalkingPhase phase;
    private float phaseTimer;
    private bool phaseEntered;

    private bool flickerStarted;
    private bool blackoutReached;

    private StareOutcome outcome;
    
    public void Enter(EntityController entity)
    {
        entity.Agent.speed = entity.StalkFollowSpeed;
        SetPhase(StalkingPhase.Following);
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
        phaseEntered = false;
    }

    private void TickHiding(EntityController entity)
    {
        bool cooldownDone = phaseTimer >= MinHideTime;
        
        // check if offscreen
        bool unseen = entity.PlayerVisibility == Visibility.Offscreen || !entity.HasLineOfSight;
        if (cooldownDone && unseen)
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
        bool seen = (entity.PlayerVisibility == Visibility.Focused || entity.PlayerVisibility == Visibility.Periphery) && entity.HasLineOfSight;
        
        if (seen)
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
        if (!phaseEntered)
        {
            phaseEntered = true;
            flickerStarted = false;
            outcome = Random.value < VanishChance ? StareOutcome.StareAndVanish : StareOutcome.Retreat;

            if (outcome == StareOutcome.StareAndVanish)
            {
               entity.Agent.ResetPath();
               entity.Agent.isStopped = true;
               entity.Agent.velocity = Vector3.zero; 
            }
        }

       switch (outcome)
        {
            case StareOutcome.Retreat: DoRetreat(entity); break;
            case StareOutcome.StareAndVanish: DoStareAndVanish(entity); break;
        }
    }

    private void DoRetreat(EntityController entity)
    {
        AINode target = entity.NodeManager.FindClosestSafeHiddenNode(entity.Player, entity.transform);
        if (target != null) entity.MoveToNode(target);
 
        entity.Agent.speed = entity.StalkHideSpeed;
        SetPhase(StalkingPhase.Hiding);
    }

    private void DoStareAndVanish(EntityController entity)
    { 
        if (!flickerStarted && phaseTimer >= StareDuration)
        {
            flickerStarted = true;
            LightingManager.Instance.Flicker(FlickerDuration, () => blackoutReached = true);
        }
 
        if (blackoutReached || phaseTimer >= MaxStareTime)
        {
            Vanish(entity);
        }
    }

    private void Vanish(EntityController entity)
    {
        entity.Agent.isStopped = false;
        entity.Agent.speed = entity.StalkHideSpeed;
 
        AINode hidden = entity.NodeManager.FindClosestSafeHiddenNode(entity.Player, entity.transform);
        if (hidden != null)
        {
            entity.CurrentTarget = hidden;
            entity.Agent.Warp(hidden.transform.position);
        }
 
        SetPhase(StalkingPhase.Hiding);
    }

    public void Exit(EntityController entity)
    {
        entity.Agent.isStopped = false;
    }

    public enum StalkingPhase
    {
        Hiding,
        Following,
        StareDown
    }

    private enum StareOutcome
    {
        Retreat,
        StareAndVanish
    }
}
