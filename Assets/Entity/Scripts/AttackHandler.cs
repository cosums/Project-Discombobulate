using System;
using UnityEngine;

/** AttackHandler is responsible for auto-attacks when the player has collided with the ghost,
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
    private Vector3 originalPosition = Vector3.zero;

    // ============= animation control ==============
    [SerializeField] private AttackAnimationHandler animHandler;
    [SerializeField] private float timeTillStartPlayerDeath = 0.5f; // TODO fine tune
    

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
        if(ghostController == null)
        {
            Debug.LogWarning("Attack check script cannot find its ghost-related refs! Attacks will not work");
            sol = true;
        }
        
    }

    private Vector3 getOriginalPosition()
    {
        return Vector3.zero; // TODO
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

    }

    private bool isAtAttackPosition()
    {
        return true; // TODO should only return true if distance to player transform is less than distanceFromPlayerWhileAttacking
    }

    private void moveTowards(Vector3 targetPos)
    {
        // TODO should steadily move towards target pos
    }

    private void startAttack()
    {
        playerDeathHandler.StartPreDeath();
        // TODO freeze ghost AI

        while (!isAtAttackPosition())
        {
            moveTowards(playerTransform.position);
        }

        animHandler.StartAnimation();
    }
}
