using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        // ✅ 상태별 라우팅
        switch (GameStateMachine.Instance.CurrentGameState)
        {
            case GameState.CutScene:
                DialogueManager.Instance.OnInput();
                break;

            case GameState.Gameplay:
                break;
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // ✅ 상태별 라우팅
        switch (GameStateMachine.Instance.CurrentGameState)
        {
            case GameState.CutScene:
                DialogueManager.Instance.OnInput();
                break;

            case GameState.Gameplay:
                break;
        }
    }

}
