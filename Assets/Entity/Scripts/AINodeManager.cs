using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINodeManager : MonoBehaviour
{
    public LayerMask VisibilityLayerMask;

    public float MaxLinkDistance = 5f;
    public float SampleInterval = 1f;

    public static List<AINode> AINodes = new();
    [HideInInspector] public List<AINode> HiddenNodes = new();

    public float BehindPlayerAngleThreshold = 90f;

    public Transform PlayerTransform;

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

                    Vector3[] corners = (Vector3[])path.corners.Clone();
                    Vector3[] samples = SamplePointsAlongPath(path.corners, SampleInterval);
                    a.Edges.Add(new AINodeEdge { Target = b, Distance = pathLength, Corners = corners, SamplePoints = samples});

                    Vector3[] reversedCorners = (Vector3[])corners.Clone();
                    Vector3[] reversedSamples = (Vector3[])samples.Clone();
                    System.Array.Reverse(reversedCorners);
                    System.Array.Reverse(reversedSamples);
                    b.Edges.Add(new AINodeEdge {Target = a, Distance = pathLength, Corners = reversedCorners, SamplePoints = reversedSamples});
                }
            }
        }
    }

    Vector3[] SamplePointsAlongPath(Vector3[] corners, float interval)
    {
        var samples = new List<Vector3>();

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 segStart = corners[i];
            Vector3 segEnd = corners[i + 1];
            float segLength = Vector3.Distance(segStart, segEnd);

            int steps = Mathf.Max(1, Mathf.CeilToInt(segLength / interval));

            for (int s = 0; s <= steps; s++)
            {
                float t = s / (float)steps;
                samples.Add(Vector3.Lerp(segStart, segEnd, t));
            }
        }

        return samples.ToArray();
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

    public float EdgeExposureCostFunc(AINode from, AINode to, float baseDistance)
    {
        AINodeEdge edge = from.Edges.Find(e => e.Target == to);
        if (edge == null || edge.Corners == null) return baseDistance;

        float worstMultiplier = 1f;

        for (int i = 0; i < edge.Corners.Length; i++)
        {
            Vector3 samplePos = edge.Corners[i] + Vector3.up * GameConstants.PlayerHeightOffset;
            Vector3 toPlayer = PlayerTransform.position - samplePos;
            float dist = toPlayer.magnitude;

            bool exposed = !Physics.Raycast(samplePos, toPlayer.normalized, dist, VisibilityLayerMask);

            if (exposed)
            {
                worstMultiplier = Mathf.Max(worstMultiplier, 10f);
            }
        }

        return baseDistance * worstMultiplier;
    }

    public float PathExposureCostFunc(AINode from, AINode to, float baseDistance)
    {
        AINodeEdge edge = from.Edges.Find(e => e.Target == to);
        if (edge == null || edge.SamplePoints == null) return baseDistance;

        foreach (var point in edge.SamplePoints)
        {
            Vector3 samplePos = point + Vector3.up * GameConstants.PlayerHeightOffset;
            Vector3 toPlayer = PlayerTransform.position - samplePos;
            float dist = toPlayer.magnitude;

            //bool los = !Physics.Raycast(samplePos, toPlayer.normalized, dist, VisibilityLayerMask);
            bool visible = VisibilityUtility.FOVCheck(samplePos, Camera.main);

            if (visible) return float.PositiveInfinity;
        }

        return baseDistance;
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
        PlayerTransform = playerTransform;
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

    public AINode FindClosestOffscreenNode(Transform playerTransform, float threshold = 0f)
    {
        AINode closestNode = FindFallbackNode();
        float closestDistance = Mathf.Infinity;
        float thresholdSqrd = threshold * threshold;

        foreach (var node in AINodes)
        {
            if (node.VisibleToPlayer) continue;
            
            float distance = (node.transform.position - playerTransform.position).sqrMagnitude;
            if (distance < thresholdSqrd) continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
}
