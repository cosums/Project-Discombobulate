using System.Collections.Generic;
using UnityEngine;

public class AINodeManager : MonoBehaviour
{
    public LayerMask VisibilityLayerMask;

    public static List<AINode> AINodes = new();

    

    public AINode FindRandHiddenNode(Transform playerTransform)
    {
        List<AINode> hiddenNodes = new();
        
        foreach (var node in AINodes)
        {
            Vector3 displacementToPlayer = playerTransform.position - node.transform.position;
            float distanceToPlayer = displacementToPlayer.magnitude;
            Vector3 dirToPlayer = displacementToPlayer.normalized;

            Vector3 castPosition = node.transform.position + Vector3.up * GameConstants.PlayerHeightOffset;

            RaycastHit hit;
            if (Physics.Raycast(castPosition, dirToPlayer, out hit, distanceToPlayer, VisibilityLayerMask)) 
            {
                hiddenNodes.Add(node);
                node.VisibleToPlayer = false;
            } else
            {
                node.VisibleToPlayer = true;
            }
        }
        
        if (hiddenNodes.Count != 0) {
            int choice = Random.Range(0, hiddenNodes.Count);
            return hiddenNodes[choice];
        } 
        else
        {
            return null;
        }
    }
}
