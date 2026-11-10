using System;
using UnityEngine;

/** AttackHandler is responsible for auto-attacks when the player has collided with the ghost,
 * for making sure the player dies when it is attacked,
 * and other ghost attack-related things
 */
public class AttackHandler : MonoBehaviour
{
    // ============= gameplay effects =============
    [SerializeField] PlayerDeathHandler playerDeathHandler = null;

    // ============= animation control ==============
    [SerializeField] private Animator animator;
    [SerializeField] private string propertyName = "PlayerIsAttacked";
    [SerializeField] private AnimationClip clip;
    private float timeTillFinished = 0;
    private float timeTillStartPlayerDeath = 0.5f; // TODO
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
        if(playerDeathHandler == null)
        {
            Debug.LogWarning("Attack check script cannot find its playerDeathHandler! Attacks will not kill the player");
            sol = true;
        }
        OnAttackAnimationFinish += () => {
            Debug.Log("Finished attack anim");
        };
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

        if(attackInProgress)
        {
            Debug.Log(timeTillFinished);
            timeTillFinished -= Time.deltaTime;

            if(timeTillFinished <= 0)
            {
                Debug.Log("guh");
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
        return true; // TODO
    }

    private void moveToAttackPosition()
    {
        // TODO
    }

    private void startAttack()
    {
        if (attackInProgress) { OnAttackAnimationFinish(); }

        attackInProgress = true;

        while(!isAtAttackPosition())
        {
            moveToAttackPosition();
        }

        timeTillFinished = clip.length;
        animator.SetTrigger(propertyName);
        OnAttackAnimationStart();
    }
}
