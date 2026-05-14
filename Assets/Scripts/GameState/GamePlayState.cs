using System.Collections.Generic;
using SongLib.Patterns.State;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GamePlayState : IState, IInputState
{
    public void OnEnter()
    {
        // PuzzleStarSpawner.CreatePuzzleStar();    
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
            return; // UI를 클릭했다면 무시하고 종료
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = GameSceneUIManager.Instance.StarCamera.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.OnInteract(); 
        }
    }
    
    public void OnRightClick()
    {
        // 우클릭이 들어오면 매니저에게 취소를 명령합니다.
        ConstellationManager.Instance.CancelDrawing();
        
        // [추가 팁] 만약 선을 다 그은 상태(isDrawing이 아닐 때)에서 우클릭을 하면
        // 가장 마지막에 그은 선을 지우는 'Undo(되돌리기)' 기능을 넣을 수도 있습니다.
    }

    public void OnSpace()
    {
        // 기존 스페이스바 로직
    }
}