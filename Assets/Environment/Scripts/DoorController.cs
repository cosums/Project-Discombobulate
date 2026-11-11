using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    // I adoor this script guys
    // reply to my code comments im so lonely
    
    [SerializeField] private Transform m_DoorPivot;
    [SerializeField] private Vector3 m_DoorOpenRotation;
    [SerializeField] private float m_Speed;

    private Vector3 _closeRotation = Vector3.zero;
    private bool _open = false;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            _open = !_open;
        }

        Vector3 targetRot = _open ? m_DoorOpenRotation : _closeRotation;
        float t = 1f - Mathf.Exp(-m_Speed * Time.deltaTime);
        m_DoorPivot.localRotation = Quaternion.Slerp(m_DoorPivot.localRotation, Quaternion.Euler(targetRot), t);
    }

    public void OpenDoor(bool open)
    {
        _open = open;
    }
}
