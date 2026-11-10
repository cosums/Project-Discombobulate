using System;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private bool isDying = false;
    private bool isResetting = false;

    // ================ Animation stuff ===================
    [SerializeField] private Animator animator;
    // this is so that the player dying looks in sync with the ghost attacking him.
    // rather than sync up animations in some fancy sophisticated way, Im just gonna delay it
    [SerializeField] private float delayBeforePlayingDeathAnimation = 0.1f; // in seconds
    private bool isAwaitingDeathStart = false;
    private float elapsedDelay = 0f;
    private bool cameraAnimIsFinished = false;
    private bool fadeToBlackIsFinished = false;
    private const string ANIM_TRIGGER_RESET = "ResetCamera";
    private const string ANIM_TRIGGER_DEATH = "PlayerDies";

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
    public void StartPreDeath()
    {
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
        elapsedDelay = 0f;
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
