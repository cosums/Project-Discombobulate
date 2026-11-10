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
    [SerializeField] Transform playerTransform = null;
    [SerializeField] private float distanceFromPlayerWhileAttacking = 1f; // TODO fine tune
    [SerializeField] private EntityController ghostController = null;
    [SerializeField] private Transform ghostRoot = null;
    private Vector3 originalPosition = Vector3.zero;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private float timeTillStartPlayerDeath = 0.5f; // TODO fine tune
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
        if(playerDeathHandler == null || playerTransform == null)
        {
            Debug.LogWarning("Attack check script cannot find its player-related refs! Attacks will not kill the player");
            sol = true;
        }
        if(ghostController == null || ghostRoot)
        {
            Debug.LogWarning("Attack check script cannot find its ghost-related refs! Attacks will not work");
            //sol = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

        if(awaitingStartOfPlayerDeath)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > timeTillStartPlayerDeath)
            {
                elapsedTime = 0f;
                awaitingStartOfPlayerDeath = false;
                playerDeathHandler.StartDeath();
            }
        }
    }

    private Vector3 getOriginalPosition()
    {
        return Vector3.zero; // TODO
    }

    private bool isAtAttackPosition()
    {
        return true; // TODO should only return true if distance to player transform is less than distanceFromPlayerWhileAttacking
    }

    private void moveTowards(Vector3 targetPos)
    {
        // TODO should steadily move towards target pos
        // quickly, & ideally with slight easing at the start & end
    }

    public void StartAttack()
    {
        // freeze player & ghost movements to take manual control
        playerDeathHandler.StartPreDeath();
        // TODO freeze ghost AI

        // move towards player
        originalPosition = getOriginalPosition();
        while (!isAtAttackPosition())
        {
            moveTowards(playerTransform.position);
        }

        // start the sequence
        animHandler.StartAnimation();
        awaitingStartOfPlayerDeath = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartAttack();
        }
    }
}
