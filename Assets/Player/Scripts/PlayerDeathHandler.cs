using System;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private bool isDying = false;
    private bool isResetting = false;

    // ================ Animation stuff ===================
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;
    private bool isAwaitingDeathStart = false;
    private bool cameraAnimIsFinished = false;
    private bool fadeToBlackIsFinished = false;
    private const string ANIM_TRIGGER_RESET = "ResetCamera";
    private const string ANIM_TRIGGER_DEATH = "PlayerDies";
    private Transform lookTransform;

    // ================== Other stuff =======================
    // TODO ref for however you reset to last save

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            if(!cameraAnimIsFinished) lookTowardsGhost();

            UpdateDeathAnimation();
            if(cameraAnimIsFinished && fadeToBlackIsFinished)
            {
                finishDeath();
            }
        }

        if(isResetting)
        {
            // TODO once previous save is loaded, call onResetFinished
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

    private void UpdateDeathAnimation()
    {
        if(isDying && !isAwaitingDeathStart)
        {
            
            
        }

        if(!isAwaitingDeathStart && cameraAnimIsFinished)
        {
            // TODO start fade to black
            // TODO check if fade to black is finished
        }
    }

    /** Should be called when an attack is initiated */
    public void StartPreDeath(Transform ghostTransform)
    {
        this.lookTransform = ghostTransform;
        isDying = true;
        isAwaitingDeathStart = true;
        PlayerController.movementDisabled = true;
        // TODO look at ghost
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
        isResetting = true;
        animator.SetTrigger(ANIM_TRIGGER_RESET);

        isDying = false;
        isAwaitingDeathStart = false;
        cameraAnimIsFinished = false;
        fadeToBlackIsFinished = false;

        // TODO load to last save
    }

    private void onResetFinished()
    {
        isResetting = false;
        PlayerController.movementDisabled = false;
        // TODO fade from black into level
    }
}
