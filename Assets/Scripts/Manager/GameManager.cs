using System;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public GameStateMachine stateMachine;

    public string CurrentChapterId;

    protected override void Awake()
    {
        base.Awake();

        stateMachine ??= FindAnyObjectByType<GameStateMachine>();
    }

    private void Start()
    {
        stateMachine.ChangeState<TitleState>();
    }
    
    public void EnterChapter(string chapterId)
    {
        CurrentChapterId = chapterId;
        Debug.Log($"Enter Chapter: {chapterId}");
    }
}
