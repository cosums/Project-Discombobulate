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
    private float _progress = 0;
    private bool _open = false;
    private bool _done = false;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _done)
        {
            _open = !_open;
            _progress = 0;
        }

        Vector3 targetRot = _open ? m_DoorOpenRotation : _closeRotation;
        Vector3 startRot = _open ? _closeRotation : m_DoorOpenRotation;
        _progress = 1f - Mathf.Exp(-m_Speed * Time.deltaTime) + _progress;
        _progress = Mathf.Min(1f, _progress);
        m_DoorPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(targetRot), _progress);

        _done = targetRot == m_DoorPivot.localEulerAngles;
    }

    public void OpenDoor(bool open)
    {
        _open = open;
    }
}
