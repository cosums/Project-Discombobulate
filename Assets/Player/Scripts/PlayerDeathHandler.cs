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
    private bool isAwaitingAnimationDelay = false;
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
    }

    private void UpdateDeathAnimation()
    {
        if(isAwaitingAnimationDelay)
        {
            elapsedDelay += Time.deltaTime;

            if (elapsedDelay > delayBeforePlayingDeathAnimation)
            {
                elapsedDelay = 0f;
                isAwaitingAnimationDelay = false;

                animator.SetTrigger(ANIM_TRIGGER_DEATH);
            }
        }

        if(!isAwaitingAnimationDelay && cameraAnimIsFinished)
        {
            // TODO start fade to black
            // TODO check if fade to black is finished
        }
    }

    public void StartDeath()
    {
        isDying = true;
        isAwaitingAnimationDelay = true;
        PlayerController.movementDisabled = true;
    }

    private void finishDeath()
    {
        isResetting = true;
        animator.SetTrigger(ANIM_TRIGGER_RESET);

        isDying = false;
        isAwaitingAnimationDelay = false;
        elapsedDelay = 0f;
        cameraAnimIsFinished = false;
        fadeToBlackIsFinished = false;

        // TODO load to last save
        // TODO once loaded, reenable controls and fade from black into level
    }

}
