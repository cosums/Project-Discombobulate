using System;
using UnityEngine;

public class SummonManager : MonoBehaviour
{
    public static event Action OnSummonStart;
    
    [SerializeField] private Collider m_CircleCollider;

    private bool _playerTooClose;

    public void OnEnable()
    {
        SaltMaster.OnSaltPlaced += HandleSaltPlace;
    }

    public void OnDisable()
    {
        SaltMaster.OnSaltPlaced += HandleSaltPlace;
    }

    private void HandleSaltPlace(Vector3 p)
    {
        transform.position = p;
    }
}
