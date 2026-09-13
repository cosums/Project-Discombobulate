using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    public Transform HeadAnchor;
    public MeshRenderer Renderer;


    public Transform Player;
    public Transform PlayerCamera;

    public Color[] colors = new Color[4];

    public NavMeshAgent Agent;

    public float CenterFOV = 30f;
    public float PeripheryFOV = 40f;


    public Visibility PlayerVisibility;
    public bool HasLineOfSight;

    public LayerMask VisibilityLayerMask;

    public AINodeManager NodeManager;
    public AINode CurrentTarget;

    [Header("Speed Controls")]
    public float StalkFollowSpeed = 0f;
    public float StalkHideSpeed = 0f;

    // behavior
    private IEntityBehavior _currentBehavior;

    public readonly StalkingBehavior StalkingBehavior = new StalkingBehavior();
    
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        ChangeBehavior(StalkingBehavior);

        NodeManager.EvaluateHiddenNodes(Player);
        CurrentTarget = NodeManager.FindRandHiddenNode(Player);
        Agent.Warp(CurrentTarget.transform.position);
    }

    void Update()
    {
        UpdateVisibility();

        LookAtPlayer();

        _currentBehavior.Tick(this);
    }

    public void ChangeBehavior(IEntityBehavior newBehavior)
    {
        _currentBehavior?.Exit(this);
        _currentBehavior = newBehavior;
        _currentBehavior?.Enter(this);
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
        } else if (VisibilityUtility.FOVCheck(transform, Camera.main))
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        if (CurrentTarget!= null) Gizmos.DrawSphere(CurrentTarget.transform.position, 0.5f);
    }
}

public enum Visibility
{
    Offscreen,
    OnScreen,
    Periphery,
    Focused
}
