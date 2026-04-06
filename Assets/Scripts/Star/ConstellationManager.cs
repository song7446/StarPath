using UnityEngine;
using System.Collections.Generic;
using SongLib.Core.Singleton; // 작성하신 네임스페이스 추가
using UnityEngine.InputSystem;

public class ConstellationManager : MonoBehaviourSingleton<ConstellationManager>
{
    [Header("Line Settings")]
    public GameObject linePrefab; // LineRenderer가 붙은 프리팹

    private PuzzleStar startStar;   
    private PuzzleStar hoveredStar; 
    private LineRenderer currentLine; 
    
    // (시작 별 ID, 끝 별 ID) 형태로 연결된 선들을 저장
    private HashSet<string> connectedPairs = new HashSet<string>();

    private void Update()
    {
        // 선을 긋는 중이면 마우스를 따라가게 함
        if (currentLine != null && startStar != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            currentLine.SetPosition(1, mousePos);
        }
    }

    // --- 이하 마우스 이벤트 처리 ---

    public void StartDrawing(PuzzleStar star)
    {
        startStar = star;
        GameObject lineObj = Instantiate(linePrefab, transform);
        currentLine = lineObj.GetComponent<LineRenderer>();
        
        currentLine.SetPosition(0, startStar.transform.position);
        currentLine.SetPosition(1, startStar.transform.position);
    }

    public void SetHoveredStar(PuzzleStar star)
    {
        hoveredStar = star;
    }

    public void ClearHoveredStar(PuzzleStar star)
    {
        if (hoveredStar == star)
        {
            hoveredStar = null;
        }
    }

    public void EndDrawing()
    {
        if (currentLine == null) return;

        // 다른 별 위에서 마우스를 뗐을 때 연결 성공
        if (hoveredStar != null && hoveredStar != startStar)
        {
            currentLine.SetPosition(1, hoveredStar.transform.position);

            // "작은번호-큰번호" 규칙으로 고유 키 생성 (방향 상관없이 같은 연결로 취급)
            int minID = Mathf.Min(startStar.starID, hoveredStar.starID);
            int maxID = Mathf.Max(startStar.starID, hoveredStar.starID);
            string pairKey = $"{minID}-{maxID}";

            if (!connectedPairs.Contains(pairKey))
            {
                connectedPairs.Add(pairKey);
                Debug.Log($"별 연결됨: {pairKey}");
                
                // 정답 체크 로직이 들어갈 자리
            }
            else
            {
                // 이미 이은 선이면 삭제
                Destroy(currentLine.gameObject);
            }
        }
        else
        {
            // 허공에 뗐으면 선 삭제
            Destroy(currentLine.gameObject);
        }

        // 초기화
        currentLine = null;
        startStar = null;
    }
}