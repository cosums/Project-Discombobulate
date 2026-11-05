using UnityEngine;

public abstract class State : MonoBehaviour
{
    [SerializeField] protected StateMachineManager stateMachine;
    [SerializeField] protected GameObject parent;
    public abstract void OnStart();
    public abstract void OnUpdate();
}