using UnityEngine;

public class AINode : MonoBehaviour
{
    public Color DebugColor = Color.white;
    public float DebugRadius = 0.25f;
    public bool VisibleToPlayer = false;

    void Start()
    {
        AINodeManager.AINodes.Add(this);
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = VisibleToPlayer ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, DebugRadius);
    }
}
