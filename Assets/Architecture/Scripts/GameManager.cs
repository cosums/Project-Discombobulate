using System;
using UnityEngine;

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
    
    public static event Action<GameState> OnStateChanged;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        EnterState(state);

        OnStateChanged?.Invoke(state);
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 0;
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
        }
    }

    private void HandleSummoning()
    {
        ChangeState(GameState.Therapy);
    }
}
