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
        // ❌ 문제의 원인: 기존 코드는 지워주세요!
        // if (EventSystem.current.IsPointerOverGameObject()) return;

        // ⭕ 해결책: 새로 만든 커스텀 함수를 호출합니다.
        if (IsPointerOverUI())
        {
            return; // UI를 클릭했다면 무시하고 종료
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.OnInteract(); 
        }
    }

    public void OnSpace()
    {
        // 기존 스페이스바 로직
    }

    // ==========================================
    // 💡 에러 없는 완벽한 UI 클릭 감지 헬퍼 함수
    // ==========================================
    private bool IsPointerOverUI()
    {
        // 1. 현재 마우스 위치 가져오기
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 2. 가짜 포인터 이벤트 생성
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;

        // 3. 해당 좌표에 있는 UI 모두 찾기
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 4. 맞은 UI가 하나라도 있으면 true
        return results.Count > 0;
    }
}