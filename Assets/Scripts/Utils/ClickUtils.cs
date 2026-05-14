using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class ClickUtils
{
    public static bool IsPointerOverUI()
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
