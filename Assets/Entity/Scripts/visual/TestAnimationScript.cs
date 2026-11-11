using UnityEngine;

public class TestAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool playerInRange = false;
    private bool last = false;
    private bool playerAttacked = false;
    private float totalTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.E))
        {
            playerInRange = !playerInRange;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAttacked = !playerAttacked;
        }*/

        totalTime += Time.deltaTime;
        if(totalTime > 3f)
        {
            playerAttacked = true;
            totalTime = 0f;
        }

        if(playerAttacked)
        {
            playerAttacked = false;
            animator.SetTrigger("PlayerIsAttacked");
        }
        if(playerInRange != last)
        {
            last = playerInRange;
            animator.SetBool("PlayerIsCloseEnoughForTense", playerInRange);
        }
    }
}
