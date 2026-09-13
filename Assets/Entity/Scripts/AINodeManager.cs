using System.Collections.Generic;
using UnityEngine;

public class AINodeManager : MonoBehaviour
{
    public LayerMask VisibilityLayerMask;

    public static List<AINode> AINodes = new();
    public List<AINode> HiddenNodes = new();

    public float BehindPlayerAngleThreshold = 90f;

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

    public AINode FindRandHiddenNode(Transform playerTransform)
    {
        EvaluateHiddenNodes(playerTransform);
        
        if (HiddenNodes.Count != 0) {
            int choice = Random.Range(0, HiddenNodes.Count);
            return HiddenNodes[choice];
        } 
        else
        {
            return null;
        }
    }

    public AINode FindFarthestHiddenNode(Transform playerTransform)
    {
        EvaluateHiddenNodes(playerTransform);

        AINode farthestNode = HiddenNodes[0];
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

        AINode best = null;
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
}
