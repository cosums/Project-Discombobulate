using System;
using System.Threading;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private bool isDying = false;
    private bool isDead = false;

    // ================ Animation stuff ===================
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip clip;
    [SerializeField] private Transform cameraTransform;
    private bool isAwaitingDeathStart = false;
    private bool cameraAnimIsFinished = false;
    private bool fadeToBlackIsFinished = false;
    private const string ANIM_TRIGGER_RESET = "ResetCamera";
    private const string ANIM_TRIGGER_DEATH = "PlayerDies";
    private Transform lookTransform;
    public Action OnReset = () => { };
    private float elapsedTime = 0f;
    [SerializeField] private FadeToBlackTransition fadeToBlackTransition;

    // ================== Other stuff =======================
    SaveManager saveManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(saveManager == null) Debug.LogWarning("PlayerDeathHandler cannot find its saveManager, deaths will not restart to last save");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("ResetCamera " + animator.GetCurrentAnimatorStateInfo(0).IsName("Static"));
        //Debug.Log("PlayerDies " + animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDeathAnimation"));
        if (isDying)
        {
            if(isAwaitingDeathStart) lookTowardsGhost();

            UpdateDeathAnimation();
            if(cameraAnimIsFinished && fadeToBlackIsFinished)
            {
                finishDeath();
            }
        }

        if(isDead)
        {
            OnReset();
        }
    }

    private void lookTowardsGhost()
    {
        if(lookTransform != null)
        {
            Vector3 direction = lookTransform.position - cameraTransform.position;
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, Quaternion.LookRotation(direction), 0.1f);
        }
    }

    private bool triggeredRestore = false;

    private void UpdateDeathAnimation()
    {
        if(isDying && !isAwaitingDeathStart)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > clip.length)
            {
                cameraAnimIsFinished = true;
            }
        }

        if(!isAwaitingDeathStart && cameraAnimIsFinished && !triggeredRestore)
        {
            GameManager.Instance.RestoreSave();
            triggeredRestore = true;
        }
    }

    /** Should be called when an attack is initiated */
    public void StartPreDeath(Transform ghostTransform)
    {
        this.lookTransform = ghostTransform;
        isDying = true;
        isAwaitingDeathStart = true;
    }

    /** Should be called when it's time to start the animation sequence */
    public void StartDeath()
    {
        if(!isAwaitingDeathStart || !isDying)
        {
            Debug.LogWarning("StartDeath was called without calling StartPreDeath first, weird shit may happen");
        }

        isAwaitingDeathStart = false;
        animator.SetTrigger(ANIM_TRIGGER_DEATH);
    }

    private void finishDeath()
    {
        isDead = true;
    }
}
