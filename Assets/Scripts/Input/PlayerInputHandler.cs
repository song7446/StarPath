using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
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

}
