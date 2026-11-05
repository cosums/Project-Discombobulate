using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaltMaster : MonoBehaviour
{
    public static event Action<Vector3> OnSaltPlaced;

    [SerializeField] private float m_InteractDistance;
    [SerializeField] private LayerMask m_InteractLayer;
    [SerializeField] private GameObject m_SaltPlaceIndicator;
    [SerializeField] private float m_FloorAngle;

    private bool _validSurface = false;

    void Update()
    {
        CheckForSurface();
        CheckForInput();
    }

    private void CheckForSurface()
    {
        m_SaltPlaceIndicator.transform.position = new Vector3(0, -1000, 0);
        _validSurface = false;
        
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, m_InteractDistance, m_InteractLayer))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            Debug.Log(angle);
            if (angle <= m_FloorAngle) {
                m_SaltPlaceIndicator.transform.position = hit.point;
                _validSurface = true;
            }
        } 
    }

    private void CheckForInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _validSurface)
        {
            OnSaltPlaced?.Invoke(m_SaltPlaceIndicator.transform.position);
        }
    }
}
