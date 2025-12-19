using SongLib.Patterns.State;
using UnityEngine;

public class CutSceneState : IState, IInputState
{
    public void OnEnter()
    {
        
    }

    public void OnUpdate(float deltaTime)
    {
        
    }

    public void OnExit()
    {
        
    }

    public void OnClick()
    {
        DialogueManager.Instance.OnInput();
    }

    public void OnSpace()
    {
        DialogueManager.Instance.OnInput();
    }
}
