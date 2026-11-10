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
    [SerializeField] private float distanceFromPlayerWhileAttacking = 2f; // TODO fine tune
    [SerializeField] private float attackApproachSpeed = 0.002f; // TODO fine tune
    [SerializeField] private EntityController ghostController = null;
    [SerializeField] private Transform ghostTransform = null;
    private Vector3 originalPosition = Vector3.zero;
    private Vector3 currentTarget;
    private const float EPSILON = 0.0001f;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private float timeTillStartPlayerDeath = 1.2f; // TODO fine tune
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
        StartAttack(); // DELETEME just for testing
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

            if (arrivedAtTargetPosition())
            {
                Debug.Log("Close enough to player with distance " + Vector3.Distance(ghostTransform.position, playerTransform.position));
                Debug.Log("Ghost pos was " + ghostTransform.position + ", Player pos was " + playerTransform.position);
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

    /** If close enough to the target attack position, returns 0. Else, returns distance to the target attack position. */
    private float compareToAttackPosition()
    {
        float distanceBetweenGhostAndPlayer = Vector3.Distance(ghostTransform.position, new Vector3(playerTransform.position.x, ghostTransform.position.y, playerTransform.position.z));
        Debug.Log("distanceBetweenGhostAndPlayer " + distanceBetweenGhostAndPlayer);
        if (Mathf.Abs(distanceBetweenGhostAndPlayer - distanceFromPlayerWhileAttacking) <= EPSILON)
        {
            Debug.Log("compared: " + (distanceBetweenGhostAndPlayer - distanceFromPlayerWhileAttacking));
            // close enough to player
            return 0f;
        }

        return distanceBetweenGhostAndPlayer - distanceFromPlayerWhileAttacking;
    }

    private void moveTowardsAttackPosition()
    {
        // get direction towards player
        Vector3 heading = Vector3.Normalize(ghostTransform.position - playerTransform.position);

        float compared = compareToAttackPosition();

        // if too close to the player, want to move backwards.
        if (compared < 1)
        {
            Debug.Log("inverting heading");
            heading = new Vector3(heading.x * -1, heading.y, heading.z * -1);
        }

        Vector3 projectedMovement = Vector3.Scale(heading, new Vector3(attackApproachSpeed, 0, attackApproachSpeed));

        // if close enough to the target pos, don't overshoot.
        // im so good at movement controllers guys
        if (Mathf.Abs(compared) < Mathf.Abs(Vector3.Magnitude(projectedMovement)))
        {
            Debug.Log("too close, ensuring no overshoot");
            projectedMovement = Vector3.Scale(heading, new Vector3(compared, 0, compared));
        }

        // finally move towards target pos
        Debug.Log("moved " + projectedMovement);
        Debug.Log("direction was " + heading + " and distance was " + Vector3.Magnitude(projectedMovement));
        ghostTransform.position = ghostTransform.position + projectedMovement;
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
        Debug.Log("currently " + ghostTransform.position + ", moving towards " + target);
        Vector3 directionFromGhostToTarget = target - ghostTransform.position;
        directionFromGhostToTarget.y = 0;
        directionFromGhostToTarget = Vector3.Normalize(directionFromGhostToTarget);
        Vector3 speedVector = new Vector3(attackApproachSpeed, 0, attackApproachSpeed);
        Vector3 movement = new Vector3(directionFromGhostToTarget.x * attackApproachSpeed, 0, directionFromGhostToTarget.z * attackApproachSpeed);

        float dist = Vector3.Magnitude(directionFromGhostToTarget);
        if (Mathf.Abs(dist) < Mathf.Abs(Vector3.Magnitude(speedVector)))
        {
            Debug.Log("dist " + dist + " was less than speed " + Mathf.Abs(Vector3.Magnitude(speedVector)));
            ghostTransform.position = new Vector3(target.x, ghostTransform.position.y, target.z);
            return;
        }

        ghostTransform.position = ghostTransform.position + movement;
    }

    public void StartAttack()
    {
        // freeze player & ghost movements to take manual control
        playerDeathHandler.StartPreDeath();
        // TODO freeze ghost AI

        // move towards player
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
