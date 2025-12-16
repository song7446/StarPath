using System;
using System.Collections.Generic;
using SongLib.Core.Singleton;
using SongLib.Patterns.State;
using UnityEngine;

public class GameStateMachine : MonoBehaviourSingleton<GameStateMachine>
{
    private StateMachine _stateMachine;
    public IState CurrentState;
    public GameState CurrentGameState;
    
    private readonly Dictionary<Type, GameState> _stateTypeMap = new()
    {
        { typeof(TitleState), GameState.Title },
        { typeof(InitState), GameState.Init },
        { typeof(GamePlayState), GameState.Gameplay },
        { typeof(CutSceneState), GameState.CutScene }
    };

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }

    public void ChangeState<T>() where T : IState, new()
    {
        _stateMachine.ChangeState<T>();
        CurrentState = _stateMachine.GetCurrentState();
        
        if (_stateTypeMap.TryGetValue(typeof(T), out var newGameState))
        {
            CurrentGameState = newGameState;
        }
        else
        {
            Debug.LogWarning($"⚠️ Unknown state type: {typeof(T).Name}");
        }

    }
}
