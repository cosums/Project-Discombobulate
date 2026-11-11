using System;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO
// - fadeIn still plays twice

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
    [SerializeField] EntityController entityController;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] private float distanceFromPlayerWhileAttacking = 1.8f;
    [SerializeField] private Transform ghostTransform = null;
    [SerializeField] private Transform headBoneTransform;
    [SerializeField] private float secondsUntilAutoStartAnimation = 1f; // sometimes something breaks with the sequence and the anim never starts :P
    private Vector3 originalPosition = Vector3.zero;
    private float elapsedMovementTime = 0f;
    private bool isAwaitingArrival;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private float timeTillStartPlayerDeath = 1.13f;
    private float elapsedTime = 0f;
    private bool awaitingStartOfPlayerDeath = false;

    private static event Action OnKillPlayer;

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
        playerDeathHandler.OnReset += PostAttackCleanup;
        if (elapsedCooldownTime >= cooldownTimeAfterAttackFinished) isOnCooldown = false;

        OnKillPlayer += StartAttack;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Keyboard.current.qKey.wasPressedThisFrame) { // DELETEME debug
            Debug.Log("q pressed triggering attack");
            StartAttack();
        }*/
        if (sol) return;

        if(isOnCooldown)
        {
            elapsedCooldownTime += Time.deltaTime;
            if(elapsedCooldownTime >= cooldownTimeAfterAttackFinished)
            {
                isOnCooldown = false;
            }
        }
        
        if(isAwaitingArrival && !arrivedAtTargetPosition)
        {
            elapsedMovementTime += Time.deltaTime;
            if(elapsedMovementTime >= secondsUntilAutoStartAnimation)
            {
                arrivedAtTargetPosition = true;
            }
        }

        // The ghost starts its attack only once it is at a good position next to the player
        if (arrivedAtTargetPosition)
        {
            isAwaitingArrival = false;
            awaitingStartOfPlayerDeath = true;
            animHandler.StartAnimation();
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

    bool arrivedAtTargetPosition = false;
    public void StartAttack()
    {
        if(isOnCooldown) {
            Debug.Log("Attack blocked due to cooldown");
            return; 
        }

        // freeze player & ghost movements to take manual control
        playerDeathHandler.StartPreDeath(headBoneTransform);
        ApproachBehavior behavior = new ApproachBehavior(distanceFromPlayerWhileAttacking, playerTransform);
        behavior.OnArriveAtTarget += () => { arrivedAtTargetPosition = true; };
        entityController.ChangeBehavior(behavior);

        // failsafe against getting stuck
        elapsedMovementTime = 0;
        isAwaitingArrival = true;

        // the rest of the sequence is handled in Update()
    }

    public void PostAttackCleanup()
    {
        entityController.ChangeBehavior(new StalkingBehavior()); // TODO change to different behaviors based on game state
        ghostTransform.position = originalPosition;
        ghostTransform.rotation = Quaternion.identity;
        resetToDefaults();
    }

    private void resetToDefaults()
    {
        elapsedTime = 0;
        elapsedMovementTime = 0;
        elapsedCooldownTime = 0f;
        awaitingStartOfPlayerDeath = false;
        isOnCooldown = true;
        isAwaitingArrival = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartAttack();
        }
    }

    public static void KillPlayer()
    {
        OnKillPlayer?.Invoke();
    }
}
