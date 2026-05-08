using SongLib.Core.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviourSingleton<PlayerInputHandler>
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (GameStateMachine.Instance.GetCurrentState() is IInputState inputState)
        {
            inputState.OnClick();
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (GameStateMachine.Instance.GetCurrentState() is IInputState inputState)
        {
            inputState.OnSpace();
        }
    }
    
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        if (GameStateMachine.Instance.GetCurrentState() is IInputState inputState)
        {
            inputState.OnRightClick();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        // 마우스가 화면에서 움직일 때마다 그 픽셀 좌표(Vector2)를 저장합니다.
        MousePosition = context.ReadValue<Vector2>();
    }
}
