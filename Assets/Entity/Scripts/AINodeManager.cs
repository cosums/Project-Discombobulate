using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINodeManager : MonoBehaviour
{
    public LayerMask VisibilityLayerMask;

    public float MaxLinkDistance = 5f;

    public static List<AINode> AINodes = new();
    [HideInInspector] public List<AINode> HiddenNodes = new();

    public float BehindPlayerAngleThreshold = 90f;

    void Awake()
    {
        AINodes = new List<AINode>(
            FindObjectsByType<AINode>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        );

        BuildGraph();
    }

    public void BuildGraph()
    {
        var path = new NavMeshPath();

        foreach (var node in AINodes) node.Edges.Clear();

        for (int i = 0; i < AINodes.Count; i++)
        {
            for (int j = i + 1; j < AINodes.Count; j++)
            {
                AINode a = AINodes[i];
                AINode b = AINodes[j];

                float absDistance = Vector3.Distance(a.transform.position, b.transform.position);
                if (absDistance > MaxLinkDistance) continue;

                if (NavMesh.CalculatePath(a.transform.position, b.transform.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = GetPathLength(path);
                    if (pathLength > MaxLinkDistance) continue;

                    a.Edges.Add(new AINodeEdge { Target = b, Distance = pathLength});
                    b.Edges.Add(new AINodeEdge {Target = a, Distance = pathLength});
                }
            }
        }
    }

    public float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    public float ExposureCostFunc(AINode from, AINode to, float baseDistance)
    {
        float multiplier = 1f;
        if (to.LineOfSightToPlayer) multiplier = 5f;
        if (to.VisibleToPlayer) multiplier = 10f;

        return baseDistance * multiplier;
    }

    public AINode FindNearestNode(Vector3 position)
    {
        AINode nearest = FindFallbackNode();
        float nearestDistance = Mathf.Infinity;

        foreach (var node in AINodes)
        {
            float distance = (node.transform.position - position).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = node;
            }
        }

        return nearest;
    }

    public void EvaluateHiddenNodes(Transform playerTransform)
    {
        HiddenNodes = new();
        
        foreach (var node in AINodes)
        {
            Vector3 displacementToPlayer = playerTransform.position - node.transform.position;
            float distanceToPlayer = displacementToPlayer.magnitude;
            Vector3 dirToPlayer = displacementToPlayer.normalized;

            Vector3 castPosition = node.transform.position + Vector3.up * GameConstants.PlayerHeightOffset;

            RaycastHit hit;
            if (Physics.Raycast(castPosition, dirToPlayer, out hit, distanceToPlayer, VisibilityLayerMask)) 
            {
                HiddenNodes.Add(node);
                node.LineOfSightToPlayer = false;
            } else
            {
                node.LineOfSightToPlayer = true;
            }
        }
    }

    public AINode FindFallbackNode()
    {
        int choice = Random.Range(0, AINodes.Count);
        return AINodes[choice];
    }

    public AINode FindRandHiddenNode(Transform playerTransform)
    {
        EvaluateHiddenNodes(playerTransform);
        
        if (HiddenNodes.Count != 0) {
            int choice = Random.Range(0, HiddenNodes.Count);
            return HiddenNodes[choice];
        } 
        else
        {
            return FindFallbackNode();
        }
    }

    public AINode FindFarthestHiddenNode(Transform playerTransform)
    {
        EvaluateHiddenNodes(playerTransform);

        AINode farthestNode = FindFallbackNode();
        float farthestDistance = 0f;

        foreach (var node in HiddenNodes)
        {
            float distance = (node.transform.position - playerTransform.position).sqrMagnitude;
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestNode = node;
            }
        }

        return farthestNode;
    }

    public AINode FindFarthestSafeHiddenNode(Transform playerTransform, Transform entityTransform)
    {
        EvaluateHiddenNodes(playerTransform);

        Vector3 dirToPlayer = (playerTransform.position - entityTransform.position).normalized;
        float distanceToPlayer = Vector3.Distance(entityTransform.position, playerTransform.position);

        AINode best = FindFallbackNode();
        float bestDistance = 0f;

        foreach (var node in HiddenNodes)
        {
            Vector3 dirToNode = (node.transform.position - entityTransform.position).normalized;
            float distanceToNode = Vector3.Distance(node.transform.position, entityTransform.position);
            float angle = Vector3.Angle(dirToPlayer, dirToNode.normalized);

            bool requiresApproachingPlayer = angle < BehindPlayerAngleThreshold && distanceToNode > distanceToPlayer;
            if (requiresApproachingPlayer) continue;

            if (distanceToNode > bestDistance)
            {
                bestDistance = distanceToNode;
                best = node;
            }
        }

        return best; // plan to use backup behavior going forward
    }

    public AINode FindClosestSafeHiddenNode(Transform playerTransform, Transform entityTransform)
    {
        EvaluateHiddenNodes(playerTransform);

        Vector3 dirToPlayer = (playerTransform.position - entityTransform.position).normalized;
        float distanceToPlayer = Vector3.Distance(entityTransform.position, playerTransform.position);

        AINode best = FindFallbackNode();
        float bestDistance = Mathf.Infinity;

        foreach (var node in HiddenNodes)
        {
            Vector3 dirToNode = (node.transform.position - entityTransform.position).normalized;
            float distanceToNode = Vector3.Distance(node.transform.position, entityTransform.position);
            float angle = Vector3.Angle(dirToPlayer, dirToNode.normalized);

            bool requiresApproachingPlayer = angle < BehindPlayerAngleThreshold && distanceToNode > distanceToPlayer;
            if (requiresApproachingPlayer) continue;

            if (distanceToNode < bestDistance)
            {
                bestDistance = distanceToNode;
                best = node;
            }
        }

        return best; // plan to use backup behavior going forward
    }

    public AINode FindClosestOffscreenNode(Transform playerTransform)
    {
        AINode closestNode = FindFallbackNode();
        float closestDistance = Mathf.Infinity;

        foreach (var node in AINodes)
        {
            if (node.VisibleToPlayer) continue;
            
            float distance = (node.transform.position - playerTransform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
}
