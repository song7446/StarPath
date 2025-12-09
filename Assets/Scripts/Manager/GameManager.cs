using System;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public GameStateMachine stateMachine;

    protected override void Awake()
    {
        base.Awake();

        stateMachine ??= FindAnyObjectByType<GameStateMachine>();
    }

    private void Start()
    {
        stateMachine.ChangeState<TitleState>();
    }
}
