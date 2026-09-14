using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPathfollower : MonoBehaviour
{
    public float ArrivalPercision = 1f; // basically how close is considered good enough to mark as good, up this value to give it

    public NavMeshAgent Agent;
    public List<AINode> CurrentPath;
    public int CurrentIndex;

    public bool IsFollowingPath => CurrentPath != null && CurrentIndex < CurrentPath.Count;

    public void SetPath(List<AINode> path)
    {
        CurrentPath = path;
        CurrentIndex = 0;

        if (CurrentPath != null && CurrentPath.Count > 0)
            Agent.SetDestination(CurrentPath[CurrentIndex].transform.position);

    }

    public void Stop()
    {
        CurrentPath = null;
        Agent.ResetPath();
    }

    void Update()
    {
        if (!IsFollowingPath) return;
        if (Agent.pathPending) return;

        if (Agent.remainingDistance <= ArrivalPercision)
        {
            CurrentIndex++;

            if (CurrentIndex < CurrentPath.Count)
            {
                Agent.SetDestination(CurrentPath[CurrentIndex].transform.position);
            } else
            {
                CurrentPath = null;
            }
        }
    }
}
 