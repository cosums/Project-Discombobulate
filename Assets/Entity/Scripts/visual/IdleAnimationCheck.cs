using System;
using UnityEngine;

public class IdleAnimationCheck : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string propertyName = "PlayerIsCloseEnoughForTense";
    private bool sol = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(this.GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Idle animation script cannot find its collider! Animations will not work");
            sol = true;
        }
        if (animator == null)
        {
            Debug.LogWarning("Idle animation script cannot find its animator! Animations will not work");
            sol = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sol) return;

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered " + other);
        // TODO test compareTag works
        if(other.CompareTag("Player"))
        {
            animator.SetBool(propertyName, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool(propertyName, false);
        }
    }
}
