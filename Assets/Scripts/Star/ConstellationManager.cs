using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using SongLib.Core.Singleton;

public class ConstellationManager : MonoBehaviourSingleton<ConstellationManager>
{
    public static ConstellationManager Instance { get; private set; }

    [Header("선 긋기 설정")] public LineRenderer linePrefab; // 에디터에서 연결할 선 프리팹

    // 상태 추적용 변수들
    private LineRenderer currentLine;
    private PuzzleStar startStar;
    private PuzzleStar hoveredStar;
    private bool isDrawing = false;

    // 완성된 선들을 모아둘 리스트
    private Stack<LineRenderer> permanentLines = new Stack<LineRenderer>();

    private void Awake()
    {
        Instance = this;
    }

    public void StartDrawing(PuzzleStar star)
    {
        isDrawing = true;
        startStar = star;

        // 1. 새로운 선 생성
        currentLine = Instantiate(linePrefab, transform);
        currentLine.positionCount = 2; // 선의 점 개수 (시작점, 끝점)

        // 2. 시작점(0)과 끝점(1)을 일단 클릭한 별의 위치로 고정
        currentLine.SetPosition(0, startStar.transform.position);
        currentLine.SetPosition(1, startStar.transform.position);
    }

    private void Update()
    {
        if (!isDrawing || currentLine == null) return;

        // 1. 마우스 월드 좌표 계산
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 2. 실시간 레이캐스트로 마우스 아래에 다른 별이 있는지 확인 (Hover 처리)
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        hoveredStar = null;

        if (hit.collider != null)
        {
            PuzzleStar star = hit.collider.GetComponent<PuzzleStar>();
            // 마우스 아래에 있는 게 별이고, 내가 처음 클릭한 시작 별이 아니라면?
            if (star != null && star != startStar)
            {
                hoveredStar = star;
            }
        }

        // 3. 선의 끝점(1번 인덱스) 위치 업데이트 (자석 효과)
        if (hoveredStar != null)
        {
            // 마우스가 다른 별 위에 있으면 마우스 좌표가 아니라 그 별의 정중앙에 선이 딱 붙게 만듭니다.
            currentLine.SetPosition(1, hoveredStar.transform.position);
        }
        else
        {
            // 허공이면 마우스 끝을 자연스럽게 따라가게 합니다.
            currentLine.SetPosition(1, worldPos);
        }

        // 4. 마우스 왼쪽 버튼을 떼는 순간 감지
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrawing();
        }
    }

    private void EndDrawing()
    {
        isDrawing = false;

        if (hoveredStar != null)
        {
            Debug.Log($"{startStar.starID}번 별과 {hoveredStar.starID}번 별 연결 완료!");

            // TODO: 나중에 여기에 ScriptableObject를 참조하여 "진짜 정답인지" 체크하는 로직이 들어갑니다.

            // 일단 연결 성공으로 간주하고 선을 유지합니다.
            permanentLines.Push(currentLine);
            currentLine = null; // 참조를 끊어서 다음 선을 그을 때 덮어씌워지지 않게 함
        }
        else
        {
            // 실패: 별이 아닌 허공에서 마우스를 뗐으므로 그리던 선을 삭제합니다.
            Destroy(currentLine.gameObject);
            currentLine = null;
        }

        // 상태 초기화
        startStar = null;
        hoveredStar = null;
    }

    public void CancelDrawing()
    {
        if (permanentLines.Count == 0) return;

        Destroy(permanentLines.Pop());
    }
}