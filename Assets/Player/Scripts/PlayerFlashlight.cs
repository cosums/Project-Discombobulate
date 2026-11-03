using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Light m_Light;
    private bool _isOn = true;

    void Update()
    {
        m_Light.enabled = _isOn;

        if (Keyboard.current.fKey.wasPressedThisFrame) _isOn = !_isOn;
    }
}
