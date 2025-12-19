using SongLib.Core.Singleton;
using SongLib.Patterns.State;
using UnityEngine;

public class GameStateMachine : MonoBehaviourSingleton<GameStateMachine>
{
    private StateMachine _stateMachine;

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
    }

    public IState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }
}