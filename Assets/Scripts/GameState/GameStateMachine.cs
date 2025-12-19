using System;
using SongLib;
using SongLib.Core.Singleton;
using SongLib.Patterns.State;
using UnityEngine;

public class GameStateMachine : MonoBehaviourSingleton<GameStateMachine>, IGameInitializer
{
    private StateMachine _stateMachine;
    private bool _initialized = false;

    public void Initialize(Action onCompleted)
    {
        _stateMachine = new StateMachine();
        IsInitialized();

        onCompleted?.Invoke();
    }

    private void Update()
    {
        if (_initialized)
            _stateMachine.Update(Time.deltaTime);
    }

    public void ChangeState<T>() where T : IState, new()
    {
        _stateMachine.ChangeState<T>();
    }

    public IState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    private void IsInitialized()
    {
        _initialized = true;
    }
}