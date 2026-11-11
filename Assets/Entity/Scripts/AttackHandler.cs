using System;
using UnityEngine;

/** AttackHandler is responsible for auto-attacks when the player has collided with the ghost,
 * for providing the means to manually attack when therapy goes wrong,
 * for making sure the player dies when it is attacked,
 * and other ghost attack-related things
 */
public class AttackHandler : MonoBehaviour
{
    private bool sol = false; // shit outta luck

    // ============= world effects =============
    [SerializeField] PlayerDeathHandler playerDeathHandler = null;

    // ============= movement control ============
    private bool awaitingArrivalAtAttackLocation = false;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] private float distanceFromPlayerWhileAttacking = 1.8f; // TODO fine tune
    [SerializeField] private float attackApproachSpeed = 0.1f;
    [SerializeField] private EntityController ghostController = null;
    [SerializeField] private Transform ghostTransform = null;
    private Vector3 originalPosition = Vector3.zero;
    private Vector3 currentTarget;
    private const float EPSILON = 0.0001f;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private Transform lookTarget = null;
    [SerializeField] private Transform playerLookTarget = null;
    [SerializeField] private float timeTillStartPlayerDeath = 1.13f;
    private float elapsedTime = 0f;
    private bool awaitingStartOfPlayerDeath = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Attack check script cannot find its collider! Auto-attack checks will not work");
            sol = true;
        }
        if (animHandler == null)
        {
            Debug.LogWarning("Attack handler cannot find its animation handler! Animations will not work");
            sol = true;
        }
        if (playerDeathHandler == null || playerTransform == null)
        {
            Debug.LogWarning("Attack check script cannot find its player-related refs! Attacks will not kill the player");
            sol = true;
        }
        if (ghostController == null || ghostTransform == null)
        {
            Debug.LogWarning("Attack check script cannot find its ghost-related refs! Attacks will not work");
            //sol = true;
        }
        // StartAttack(); // DELETEME just for testing
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

        // The ghost starts its attack only once it is at a good position next to the player
        if (awaitingArrivalAtAttackLocation)
        {
            //moveTowardsAttackPosition();
            moveTowardsPosition(currentTarget);
            rotateTowardsTarget();

            if (arrivedAtTargetPosition())
            {
                // start the sequence
                awaitingArrivalAtAttackLocation = false;
                awaitingStartOfPlayerDeath = true;
                animHandler.StartAnimation();
            }
        }

        // An attack triggers the player's death animation once the ghost's animation is partially complete
        if (awaitingStartOfPlayerDeath)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > timeTillStartPlayerDeath)
            {
                elapsedTime = 0f;
                awaitingStartOfPlayerDeath = false;
                playerDeathHandler.StartDeath();
            }
        }
    }

    private void setOriginalPosition()
    {
        originalPosition = ghostTransform.position;
    }

    private bool arrivedAtTargetPosition()
    {
        return Vector3.Magnitude(ghostTransform.position - currentTarget) < EPSILON;
    }

    private Vector3 getTargetAttackPosition(Vector3 ghostPos, Vector3 playerPos)
    {
        Vector3 directionFromPlayerToGhost = ghostPos - playerPos;
        directionFromPlayerToGhost.y = 0;
        Vector3 scaled = Vector3.Scale(Vector3.Normalize(directionFromPlayerToGhost), 
            new Vector3(distanceFromPlayerWhileAttacking, 1, distanceFromPlayerWhileAttacking));

        return scaled + new Vector3(playerPos.x, ghostPos.y, playerPos.z);
    }

    private void moveTowardsPosition(Vector3 target)
    {
        Vector3 cur = ghostTransform.position;
        ghostTransform.position = Vector3.MoveTowards(cur, target, attackApproachSpeed);
    }

    private void rotateTowardsTarget()
    {
        Vector3 direction = playerTransform.position - ghostTransform.position;
        direction.y = 0;
        ghostTransform.rotation = Quaternion.Slerp(ghostTransform.rotation, Quaternion.LookRotation(direction), 0.05f);
        Vector3 euler = ghostTransform.eulerAngles; // idk where its getting that x rotation from but this removes it
        ghostTransform.rotation = Quaternion.Euler(new Vector3(0, euler.y, 0));
    }

    public void StartAttack()
    {
        // freeze player & ghost movements to take manual control
        playerDeathHandler.StartPreDeath(playerLookTarget);
        // TODO freeze ghost AI

        // move towards player
        if (lookTarget != null) ghostTransform.LookAt(lookTarget);
        setOriginalPosition();
        currentTarget = getTargetAttackPosition(ghostTransform.position, playerTransform.position);
        awaitingArrivalAtAttackLocation = true;

        // the rest of the sequence is handled in Update()
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartAttack();
        }
    }
}
