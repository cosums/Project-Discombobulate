using System.Collections.Generic;
using UnityEngine;

public class StalkingBehavior : IEntityBehavior
{
    private const float VanishChance = 0.25f;      
    private const float StareDuration = 2f;
    private const float FlickerDuration = 1.2f;
    private const float CutStareDuration = 0.75f;
    private const float CutHoldDuration = 0.8f;
    private const float MaxStareTime = 6f;

    private const float MinHideTime = 3f;
    private const float VanishMinDistance = 10f;
    private const float RepathInterval = 0.5f;
    private const int MaxFollowAttempts = 3;

    private StalkingPhase phase;
    private float phaseTimer;
    private bool phaseEntered;
    private float repathTimer;

    private StareOutcome outcome;
    private StareOutcome? forcedOutcome;
    private AINode retreatNode;
    private bool lightsTriggered;
    private bool blackoutReached;

    private readonly List<AINode> followCandidates = new();

    public void Enter(EntityController entity)
    {
        entity.Agent.speed = entity.StalkFollowSpeed;
        forcedOutcome = null;
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

    public void Exit(EntityController entity)
    {
        entity.Agent.isStopped = false;
    }

    private void SetPhase(StalkingPhase p)
    {
        phase = p;
        phaseTimer = 0f;
        phaseEntered = false;
        repathTimer = 0f;
    }

    private void TickHiding(EntityController entity)
    {
        var nodes = entity.NodeManager;
        bool unseen = entity.PlayerVisibility == Visibility.Offscreen || !entity.HasLineOfSight;

        if (phaseTimer >= MinHideTime && unseen && nodes.HiddenNodes.Count > 0)
        {
            AINode next = nodes.FindRandHiddenNode(entity.Player);
            entity.CurrentTarget = next;
            entity.Agent.Warp(next.transform.position);
            entity.Agent.speed = entity.StalkFollowSpeed;
            SetPhase(StalkingPhase.Following);
            return;
        }

        AINode spot = entity.CurrentTarget;
        if (spot != null && spot.VisibleToPlayer && spot.LineOfSightToPlayer)
        {
            repathTimer -= Time.deltaTime;
            if (repathTimer > 0f) return;
            repathTimer = RepathInterval;

            AINode retreat = nodes.FindRetreatNode(entity.Player, entity.transform);
            if (retreat != null)
            {
                entity.MoveToNode(retreat);
            }
            else
            {
                forcedOutcome = StareOutcome.CutLights;
                SetPhase(StalkingPhase.StareDown);
            }
        }
    }

    private void TickFollowing(EntityController entity)
    {
        bool seen = (entity.PlayerVisibility == Visibility.Focused ||
                     entity.PlayerVisibility == Visibility.Periphery) && entity.HasLineOfSight;
        if (seen)
        {
            SetPhase(StalkingPhase.StareDown);
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;
        repathTimer = RepathInterval;

        entity.NodeManager.GetFollowCandidates(
            entity.Player,
            entity.StalkMinDistance, entity.StalkTargetDistance, entity.StalkMaxDistance,
            entity.CurrentTarget, followCandidates);

        int attempts = Mathf.Min(MaxFollowAttempts, followCandidates.Count);
        for (int i = 0; i < attempts; i++)
        {
            AINode node = followCandidates[i];

            if (node == entity.CurrentTarget) return;

            if (entity.SmartMoveToNode(node, entity.NodeManager.PathExposureCostFunc)) return;
        }

        entity.Pathfollower.Stop();
        entity.CurrentTarget = null;
    }

    private void TickStareDown(EntityController entity)
    {
        if (!phaseEntered)
        {
            phaseEntered = true;
            lightsTriggered = false;
            blackoutReached = false;
            retreatNode = null;

            if (forcedOutcome.HasValue)
            {
                outcome = forcedOutcome.Value;
                forcedOutcome = null;
            }
            else
            {
                retreatNode = entity.NodeManager.FindRetreatNode(entity.Player, entity.transform);

                if (retreatNode == null)
                    outcome = StareOutcome.CutLights;
                else
                    outcome = Random.value < VanishChance ? StareOutcome.StareAndVanish : StareOutcome.Retreat;
            }

            if (outcome != StareOutcome.Retreat) entity.Freeze();
        }

        switch (outcome)
        {
            case StareOutcome.Retreat: DoRetreat(entity); break;
            case StareOutcome.StareAndVanish: DoStareThenLights(entity, StareDuration, flicker: true); break;
            case StareOutcome.CutLights: DoStareThenLights(entity, CutStareDuration, flicker: false); break;
        }
    }

    private void DoRetreat(EntityController entity)
    {
        entity.Agent.speed = entity.StalkHideSpeed;
        entity.MoveToNode(retreatNode); 
        SetPhase(StalkingPhase.Hiding);
    }

    private void DoStareThenLights(EntityController entity, float stareTime, bool flicker)
    {
        if (!lightsTriggered && phaseTimer >= stareTime)
        {
            lightsTriggered = true;

            var lights = LightingManager.Instance;
            if (lights == null) blackoutReached = true; 
            else if (flicker) lights.Flicker(FlickerDuration, () => blackoutReached = true);
            else lights.Blackout(CutHoldDuration, () => blackoutReached = true);
        }

        if (blackoutReached || phaseTimer >= MaxStareTime)
        {
            Vanish(entity);
        }
    }

    private void Vanish(EntityController entity)
    {
        entity.Pathfollower.Stop();
        entity.Agent.isStopped = false;
        entity.Agent.speed = entity.StalkHideSpeed;

        AINode spot = entity.NodeManager.FindVanishNode(entity.Player, VanishMinDistance);
        if (spot != null)
        {
            entity.CurrentTarget = spot;
            entity.Agent.Warp(spot.transform.position);
        }

        SetPhase(StalkingPhase.Hiding);
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
        StareAndVanish,
        CutLights 
    }
}
