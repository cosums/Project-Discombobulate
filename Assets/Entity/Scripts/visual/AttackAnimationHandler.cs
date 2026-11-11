using System;
using UnityEngine;

public class AttackAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string propertyName = "PlayerIsAttacked";
    [SerializeField] private AnimationClip clip;
    private float timeTillFinished = 0;
    public Action OnAttackAnimationStart = () => { };
    public Action OnAttackAnimationFinish = () => { };
    private bool attackInProgress = false;
    private bool sol = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        if (attackInProgress)
        {
            //Debug.Log(timeTillFinished);
            timeTillFinished -= Time.deltaTime;

            if (timeTillFinished <= 0)
            {
                OnAttackAnimationFinish();
                attackInProgress = false;
            }
        }
    }

    public void StartAnimation()
    {
        if (attackInProgress) { OnAttackAnimationFinish(); }
        attackInProgress = true;
        timeTillFinished = clip.length;
        animator.SetTrigger(propertyName);
        OnAttackAnimationStart();
    }
}
