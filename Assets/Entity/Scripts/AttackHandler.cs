using System;
using UnityEngine;

/** AttackHandler is responsible for auto-attacks when the player has collided with the ghost,
 * for making sure the player dies when it is attacked,
 * and other ghost attack-related things
 */
public class AttackHandler : MonoBehaviour
{
    // ============= world effects =============
    [SerializeField] PlayerDeathHandler playerDeathHandler = null;
    [SerializeField] Transform playerTransform = null;
    [SerializeField] private float distanceFromPlayerWhileAttacking = 1f; // TODO fine tune
    [SerializeField] private EntityController ghostController = null;
    private Vector3 originalPosition = Vector3.zero;

    // ============= animation control ==============
    [SerializeField] private Animator animator;
    [SerializeField] private string propertyName = "PlayerIsAttacked";
    [SerializeField] private AnimationClip clip;
    private float timeTillFinished = 0;
    [SerializeField] private float timeTillStartPlayerDeath = 0.5f; // TODO fine tune
    public Action OnAttackAnimationStart = () => { };
    public Action OnAttackAnimationFinish = () => { };
    private bool attackInProgress = false;
    private bool sol = false; // shit outta luck

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Attack check script cannot find its collider! Auto-attack checks will not work");
            sol = true;
        }
        if (animator == null)
        {
            Debug.LogWarning("Attack check script cannot find its animator! Animations will not work");
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
        OnAttackAnimationFinish += () => {
            Debug.Log("Finished attack anim");
        };
        
        
    }

    private Vector3 getOriginalPosition()
    {
        return Vector3.zero; // TODO
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

        if(attackInProgress)
        {
            Debug.Log(timeTillFinished);
            timeTillFinished -= Time.deltaTime;

            if(timeTillFinished < timeTillStartPlayerDeath)
            {
                playerDeathHandler.StartDeath();
            }

            if(timeTillFinished <= 0)
            {
                // we attacked but the attack anim is no longer playing, so it is now finished\
                OnAttackAnimationFinish();
                attackInProgress = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            startAttack();
        }
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
        if (attackInProgress) { OnAttackAnimationFinish(); }

        attackInProgress = true;
        playerDeathHandler.StartPreDeath();
        // TODO freeze ghost AI

        while (!isAtAttackPosition())
        {
            moveTowards(playerTransform.position);
        }

        timeTillFinished = clip.length;
        animator.SetTrigger(propertyName);
        OnAttackAnimationStart();
    }
}
