using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private bool _paused = false;
    public GameObject SettingsPanel;

    void Start()
    {
        SettingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_paused && GameManager.Instance.CurrentState == GameState.Playing)
            {
                GameManager.Instance.ChangeState(GameState.Paused);
                SetPause(true);
                _paused = true;
            }
            else if (_paused && GameManager.Instance.CurrentState == GameState.Paused)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
                SetPause(false);
                _paused = false;
            }
        }
    }

    private void SetPause(bool toPause)
    {
        SettingsPanel.SetActive(toPause);
    }

    public void Resume()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
        SetPause(false);
        _paused = false;
    }

    public void Reload()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
        GameManager.Instance.RestoreSave();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
