using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { 
    MainMenu = 0, 
    Playing = 1, 
    Paused = 2,
    Therapy = 3
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }
    
    public static event Action<GameState> OnStateExited;
    public static event Action<GameState> OnStateEntered;

    [SerializeField] private string m_StartScene;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // for debug purposes!
            if (SceneManager.GetActiveScene().name != m_StartScene)
            {
                ChangeState(GameState.Playing);
            }

        } else
        {
            Debug.LogWarning(">1 Game Manager in scene!");
            Destroy(gameObject);
        }
    }

    public void OnEnable() 
    {
        SummonManager.OnSummonStart += HandleSummoning;
    }

    public void OnDisable()
    {
        SummonManager.OnSummonStart -= HandleSummoning;
    }

    public void ChangeState(GameState state)
    {
        Debug.Log("[GameManager]: Changing state to " + state);
        
        ExitState(state);
        OnStateExited?.Invoke(state);

        CurrentState = state;

        EnterState(state);
        OnStateEntered?.Invoke(state);
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.Therapy:
                UIManager.DisableCursor();
                break;
        }
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 1;
                break;
            case GameState.Therapy:
                UIManager.EnableCursor();
                break;
        }
    }

    private void HandleSummoning()
    {
        ChangeState(GameState.Therapy);
    }
}
