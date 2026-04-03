using UnityEngine;

public class PuzzleStar : MonoBehaviour
{
    public int starID; // 노트의 정답과 비교할 고유 번호
    public bool isCorrect;
    
    public void SetupStar(int id, bool correct)
    {
        starID = id;
        isCorrect = correct;
        
        // 정답 별인지 방해 별인지에 따라 색상이나 크기를 살짝 다르게 할 수도 있음
        if(!isCorrect) { /* 방해 별 설정 */ }
    }

    private void OnMouseDown()
    {
        // 클릭 시작: 선 긋기 시작
        ConstellationManager.Instance.StartDrawing(this);
    }

    private void OnMouseEnter()
    {
        // 마우스가 별 위에 올라옴: 도착 지점 후보 등록
        ConstellationManager.Instance.SetHoveredStar(this);
    }

    private void OnMouseExit()
    {
        // 마우스가 별에서 벗어남: 도착 지점 후보 해제
        ConstellationManager.Instance.ClearHoveredStar(this);
    }

    private void OnMouseUp()
    {
        // 클릭 해제: 선 긋기 종료 및 연결 확정
        ConstellationManager.Instance.EndDrawing();
    }
}