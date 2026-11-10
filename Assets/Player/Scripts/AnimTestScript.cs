using UnityEngine;

// This script repeatedly triggers two triggers for an animation controller, alternating, each one after 5 seconds
public class AnimTestScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string firstTrigger = "PlayerDies";
    [SerializeField] private string secondTrigger = "ResetCamera";
    private bool playedFirst = false;
    private float totalTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if (totalTime > 5f && !playedFirst)
        {
            playedFirst = true;
            animator.SetTrigger(firstTrigger);
            totalTime = 0f;
        }
        if (totalTime > 5f && playedFirst)
        {
            playedFirst = false;
            animator.SetTrigger(secondTrigger);
            totalTime = 0f;
        }
    }
}
