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
                // StarDrawingManager.Instance.OnClick();
                break;

            case GameState.UI:
                // UI는 EventSystem이 이미 처리 중이므로 무시
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
                if (CutsceneManager.Instance.IsWaitingForInput)
                    CutsceneManager.Instance.ContinueCutscene();
                break;

            case GameState.Gameplay:
                break;

            case GameState.UI:
                // UI는 EventSystem이 이미 처리 중이므로 무시
                break;
        }
    }

}
