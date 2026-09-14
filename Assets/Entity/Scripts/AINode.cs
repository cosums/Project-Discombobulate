using System.Collections.Generic;
using UnityEngine;

public class AINode : MonoBehaviour
{
    public Color DebugColor = Color.white;
    public float DebugRadius = 0.25f;
    public bool VisibleToPlayer = false;
    public bool LineOfSightToPlayer = false;
    private Camera _PlayerCamera;
    public List<AINodeEdge> Edges;

    void Start()
    {
        _PlayerCamera = Camera.main;
    }

    void Update()
    {
        VisibleToPlayer = CheckVisibility();
    }

    void OnDrawGizmos()
    {
        Color targetColor;
        
        if (LineOfSightToPlayer && VisibleToPlayer)
        {
            targetColor = Color.red;
        } else if (LineOfSightToPlayer)
        {
            targetColor = Color.orange;
        } else 
        {
            targetColor = Color.green;
        } 

        Gizmos.color = targetColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * GameConstants.PlayerHeightOffset, DebugRadius);

        foreach (var edge in Edges)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up * GameConstants.PlayerHeightOffset, edge.Target.transform.position +  Vector3.up * GameConstants.PlayerHeightOffset);
            foreach (var corner in edge.Corners)
            {
                Gizmos.color = Color.lightGray;
                Gizmos.DrawSphere(corner, 0.2f);
            }
            foreach (var sample in edge.SamplePoints)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(sample, 0.1f);
            }
        }

        
    }

    private bool CheckVisibility()
    {
        return VisibilityUtility.FOVCheck(transform, _PlayerCamera);
    }
}

[System.Serializable]
public class AINodeEdge
{
    public AINode Target;
    public float Distance;
    public Vector3[] Corners;
    public Vector3[] SamplePoints;
}