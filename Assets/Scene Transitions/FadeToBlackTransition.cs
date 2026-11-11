using System;
using UnityEngine;

public class FadeToBlackTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip clipTo;
    [SerializeField] private AnimationClip clipFrom;
    private bool playingTo = false;
    private bool playingFrom = false;
    private float elapsedTime = 0f;
    public Action OnTransitionToFinished = () => { };
    public Action OnTransitionToStarted = () => { };
    public Action OnTransitionFromFinished = () => { };
    public Action OnTransitionFromStarted = () => { };

    public void FadeToBlack()
    {
        if (playingFrom) { finishFrom(); }
        animator.SetTrigger("TriggerFadeToBlack");
        OnTransitionToStarted();
        playingTo = true;
    }

    public void FadeFromBlack()
    {
        if(playingTo) { finishTo(); }
        animator.SetTrigger("TriggerFadeFromBlack");
        OnTransitionFromStarted();
        playingFrom = true;
    }

    private void finishTo()
    {
        elapsedTime = 0f;
        playingTo = false;
        OnTransitionToFinished();
    }

    private void finishFrom()
    {
        elapsedTime = 0f;
        playingFrom = false;
        OnTransitionFromFinished();
    }

    private void Update()
    {
        if(playingTo)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > clipTo.length)
            {
                finishTo();
            }
        }

        if (playingFrom)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > clipFrom.length)
            {
                finishFrom();
            }
        }
    }
}
