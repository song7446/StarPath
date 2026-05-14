using System.Collections.Generic;
using SongLib.Patterns.State;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        if (ClickUtils.IsPointerOverUI())
        {
            return;
        }
        DialogueManager.Instance.OnInput();
    }

    public void OnSpace()
    {
        DialogueManager.Instance.OnInput();
    }
}
