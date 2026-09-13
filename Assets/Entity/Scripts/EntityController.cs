using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    public Transform HeadAnchor;
    public MeshRenderer Renderer;


    public Transform Player;
    public Transform PlayerCamera;

    public Color[] colors = new Color[4];

    private NavMeshAgent m_Agent;

    public float CenterFOV = 30f;
    public float PeripheryFOV = 40f;

    public BehaviorMode BehaviorState = BehaviorMode.Stalking;
    public Visibility PlayerVisibility;
    public bool HasLineOfSight;

    public LayerMask VisibilityLayerMask;

    public AINodeManager NodeManager;
    
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();

        AINode startNode = NodeManager.FindRandHiddenNode(Player);

        m_Agent.Warp(startNode.transform.position);

    }

    void Update()
    {
        UpdateVisibility();

        LookAtPlayer();
    }

    void UpdateVisibility()
    {
        Vector3 displacementToPlayer = PlayerCamera.position - HeadAnchor.position;
        float distanceToPlayer = displacementToPlayer.magnitude;
        Vector3 dirToPlayer = displacementToPlayer.normalized;
        float angle = Vector3.Angle(PlayerCamera.transform.forward, -dirToPlayer);
        
        if (angle < CenterFOV)
        {
            PlayerVisibility = Visibility.Focused;
            Renderer.material.color = colors[0];
        } else if (angle < PeripheryFOV)
        {
            PlayerVisibility = Visibility.Periphery;
            Renderer.material.color = colors[1];
        } else if (Renderer.isVisible)
        {
            PlayerVisibility = Visibility.OnScreen;
            Renderer.material.color = colors[2];
        } else
        {
            PlayerVisibility = Visibility.Offscreen;
            Renderer.material.color = colors[3];
        }

        RaycastHit hit;
        if (Physics.Raycast(HeadAnchor.position, dirToPlayer, out hit, distanceToPlayer, VisibilityLayerMask))
        {
            HasLineOfSight = false;
            Debug.DrawRay(transform.position, dirToPlayer * hit.distance, Color.red);
        } else
        {
            HasLineOfSight = true;
            Debug.DrawRay(transform.position, dirToPlayer * distanceToPlayer, Color.green);
        }
    }

    void LookAtPlayer()
    {
        if (PlayerVisibility == Visibility.Focused) return;
        HeadAnchor.LookAt(PlayerCamera.position);
    }
}

public enum BehaviorMode
{
    Stalking
}

public enum StalkingPhase
{
    Hiding,
    Following,
    StareDown
}

public enum Visibility
{
    Offscreen,
    OnScreen,
    Periphery,
    Focused
}
