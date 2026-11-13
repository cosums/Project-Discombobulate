using System;
using UnityEngine;

public class SummonManager : MonoBehaviour
{
    public static event Action OnSummonStart;
    
    [SerializeField] private Transform m_PlayerTransform;
    [SerializeField] private float m_SafeSummonDistance;
    private bool _playerTooClose;
    private bool _trySummon;

    public void OnEnable()
    {
        SaltMaster.OnSaltPlaced += HandleSaltPlace;
    }

    public void OnDisable()
    {
        SaltMaster.OnSaltPlaced -= HandleSaltPlace;
    }

    public void Update()
    {
        _playerTooClose = Vector3.Distance(m_PlayerTransform.position, transform.position) <= m_SafeSummonDistance;

        if (_trySummon && !_playerTooClose)
        {
            OnSummonStart?.Invoke();
            _trySummon = false;
        }
    }
    private void HandleSaltPlace(Vector3 p)
    {
        transform.position = p;
        _trySummon = true;
    }
}
