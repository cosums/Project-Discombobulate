using System;
using UnityEngine;

public class AutoAttackCheck : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string propertyName = "PlayerIsAttacked";
    private string ANIM_NAME_1 = "Attack_FromRelaxed";
    private string ANIM_NAME_2 = "Attack_FromTense";
    public Action OnAttackAnimationStart = () => { };
    public Action OnAttackAnimationFinish = () => { };
    private bool attackInProgress = false;
    private bool sol = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Attack check script cannot find its collider! Attack checks will not work");
            sol = true;
        }
        if (animator == null)
        {
            Debug.LogWarning("Attack check script cannot find its animator! Animations will not work");
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
            // TODO this shi dont work
            AnimatorStateInfo i = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(i.fullPathHash);
            if (!i.IsName(ANIM_NAME_1) && !i.IsName(ANIM_NAME_2) && !animator.IsInTransition(0))
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
            if (attackInProgress) { OnAttackAnimationFinish(); }
            attackInProgress = true;
            animator.SetTrigger(propertyName);
            OnAttackAnimationStart();
        }
    }
}
