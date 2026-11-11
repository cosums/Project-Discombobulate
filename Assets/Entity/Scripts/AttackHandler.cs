using System;
using UnityEngine;

// TODO
// - player camera animation is now too low
// - animation doesnt get reset correctly
// - sometimes attack animation just doesnt start
// - movementDisabled might be insufficient to disable movement if entity is within a loop somewhere

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
    [SerializeField] float cooldownTimeAfterAttackFinished = 2f;
    private bool isOnCooldown = true;
    private float elapsedCooldownTime = 0f;

    // ============= movement control ============
    private bool awaitingArrivalAtAttackLocation = false;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] private float distanceFromPlayerWhileAttacking = 1.8f;
    [SerializeField] private float attackApproachSpeed = 0.1f;
    [SerializeField] private Transform ghostTransform = null;
    private Vector3 originalPosition = Vector3.zero;
    private Vector3 currentTarget;
    private const float EPSILON = 0.0001f;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private Transform lookTarget = null;
    [SerializeField] private Transform headTurnTarget = null;
    [SerializeField] private Transform headBoneTransform = null;
    [SerializeField] private float timeTillStartPlayerDeath = 1.13f;
    private float elapsedTime = 0f;
    private bool awaitingStartOfPlayerDeath = false;
    private bool shouldRotateHead = false;
    //private float animatorSpeed = 0f;


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
        if (ghostTransform == null)
        {
            Debug.LogWarning("Attack check script cannot find its ghost-related refs! Attacks will not work");
            //sol = true;
        }
        animHandler.OnAttackAnimationFinish += () => { shouldRotateHead = true; };
        playerDeathHandler.OnReset += PostAttackCleanup;
        if (elapsedCooldownTime >= cooldownTimeAfterAttackFinished) isOnCooldown = false;
        //StartAttack(); // DELETEME just for testing
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

        if(isOnCooldown)
        {
            elapsedCooldownTime += Time.deltaTime;
            if(elapsedCooldownTime >= cooldownTimeAfterAttackFinished)
            {
                isOnCooldown = false;
            }
        }

        // The ghost starts its attack only once it is at a good position next to the player
        if (awaitingArrivalAtAttackLocation)
        {
            //moveTowardsAttackPosition();
            moveTowardsPosition(currentTarget);
            rotateTowardsTarget(ghostTransform, playerTransform);

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

        // Looks at the downed player with just her head
        if (shouldRotateHead)
        {
            rotateTowardsTarget(headBoneTransform, headTurnTarget);
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

    // TODO doesnt work, probably because the animator has control
    private void rotateTowardsTarget(Transform self, Transform target)
    {
        Vector3 direction = target.position - self.position;
        direction.y = 0;
        self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(direction), 0.05f);
        Vector3 euler = self.eulerAngles; // idk where its getting that x rotation from but this removes it
        self.rotation = Quaternion.Euler(new Vector3(0, euler.y, 0));
    }

    public void StartAttack()
    {
        if(isOnCooldown) {
            Debug.Log("Attack blocked due to cooldown");
            return; 
        }

        // freeze player & ghost movements to take manual control
        playerDeathHandler.StartPreDeath(headBoneTransform);
        EntityController.movementDisabled = true;

        // move towards player
        if (lookTarget != null) ghostTransform.LookAt(lookTarget);
        setOriginalPosition();
        currentTarget = getTargetAttackPosition(ghostTransform.position, playerTransform.position);
        awaitingArrivalAtAttackLocation = true;

        // the rest of the sequence is handled in Update()
    }

    public void PostAttackCleanup()
    {
        EntityController.movementDisabled = false;
        ghostTransform.position = originalPosition;
        ghostTransform.rotation = Quaternion.identity;
        resetToDefaults();
    }

    private void resetToDefaults()
    {
        awaitingArrivalAtAttackLocation = false;
        elapsedTime = 0;
        awaitingStartOfPlayerDeath = false;
        shouldRotateHead = false;
        isOnCooldown = true;
        elapsedCooldownTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartAttack();
        }
    }
}
