using UnityEngine;

public class AINode : MonoBehaviour
{
    public Color DebugColor = Color.white;
    public float DebugRadius = 0.25f;
    public bool VisibleToPlayer = false;
    public bool LineOfSightToPlayer = false;

    private Camera _PlayerCamera;

    void Start()
    {
        AINodeManager.AINodes.Add(this);
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
            targetColor = Color.green;
        } else if (LineOfSightToPlayer)
        {
            targetColor = Color.yellow;
        } else if (VisibleToPlayer)
        {
            targetColor = Color.orange;
        } else
        {
            targetColor = Color.red;
        }

        Gizmos.color = targetColor;

        Gizmos.DrawSphere(transform.position + Vector3.up * GameConstants.PlayerHeightOffset, DebugRadius);
    }

    private bool CheckVisibility()
    {
        return VisibilityUtility.FOVCheck(transform, _PlayerCamera);
    }
}
