using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib.Core.Singleton;

public class ConstellationManager : MonoBehaviourSingleton<ConstellationManager>
{
    [Header("선 긋기 설정")] public LineRenderer linePrefab;

    // 상태 추적용 변수들
    private LineRenderer currentLine;
    private PuzzleStar startStar;
    private PuzzleStar hoveredStar;
    private bool isDrawing = false;

    // 완성된 선들을 모아둘 리스트
    private Stack<LineRenderer> permanentLines = new Stack<LineRenderer>();

    private ConstellationData currentAnswerData;
    public List<StarConnection> currentConnections = new List<StarConnection>();
    
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
        Vector2 worldPos = GameSceneUIManager.Instance.StarCamera.ScreenToWorldPoint(mousePos);

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
            // 수정 1: 전역 변수 재사용 금지! 매번 새로운 객체를 찍어내야 합니다.
            StarConnection newConnection = new StarConnection();
            newConnection.starA = startStar.starID;
            newConnection.starB = hoveredStar.starID;

            Debug.Log($"{startStar.starID}번 별과 {hoveredStar.starID}번 별 연결 완료!");

            // [추가 팁] 이미 그은 선(중복)이라면 무시하는 방어 로직
            bool alreadyExists = currentConnections.Exists(c => c.IsSameConnection(newConnection.starA, newConnection.starB));
            if (!alreadyExists)
            {
                currentConnections.Add(newConnection);
                permanentLines.Push(currentLine);
                
                CheckAnswer(); // 정답 체크 실행
            }
            else
            {
                // 이미 연결된 선이면 방금 그은 시각적 선 파괴
                Destroy(currentLine.gameObject);
            }
        }
        else
        {
            // 아무 별에도 안 닿고 마우스를 뗐으면 선 파괴
            Destroy(currentLine.gameObject);
        }

        currentLine = null; // 선 참조 초기화

        // 상태 초기화
        startStar = null;
        hoveredStar = null;
    }

    private async Task CheckAnswer()
    {
        if (currentAnswerData == null || currentAnswerData.correctConnections == null) return;

        bool isWrongLineDrawn = false;

        // 1. 내가 그은 선들이 정답 데이터 안에 '존재하는 선'인지 검사
        foreach (var drawnConn in currentConnections)
        {
            bool matchFound = false;
            foreach (var answerConn in currentAnswerData.correctConnections)
            {
                // IsSameConnection이 방향 상관없이 같은 연결인지(A-B == B-A) 체크해준다고 가정합니다.
                if (answerConn.IsSameConnection(drawnConn.starA, drawnConn.starB))
                {
                    matchFound = true;
                    break;
                }
            }

            // 정답에 없는 엄한 선을 그었다면 즉시 오답
            if (!matchFound)
            {
                isWrongLineDrawn = true;
                break;
            }
        }

        // 2. 판정 결과 출력
        if (isWrongLineDrawn)
        {
            Debug.Log("오답: 잘못된 연결이 포함되어 있습니다!");
            // (선택) 여기서 모든 선을 초기화하거나, 플레이어에게 틀렸다는 피드백을 줄 수 있습니다.
        }
        else if (currentConnections.Count == currentAnswerData.correctConnections.Count)
        {
            // 잘못된 선도 없고, 그은 선의 개수도 정답과 똑같다면? (모양 완벽 일치!)
            Debug.Log("★ 정답! 별자리를 완벽하게 완성했습니다! ★");

            //  TODO:스테이지 클리어 이벤트 호출
			await GameManager.Instance.EnterNextChapter();
        }
        else
        {
            Debug.Log($"진행 중... ({currentConnections.Count}/{currentAnswerData.correctConnections.Count})");
        }
    }

    public void CancelDrawing()
    {
        // 취소할 선이 없다면 리턴
        if (permanentLines.Count == 0) return;

        // 수정 3: 화면에서 선을 파괴하는 것뿐만 아니라...
        LineRenderer lineToRemove = permanentLines.Pop();
        Destroy(lineToRemove.gameObject);

        // 논리적 데이터(리스트)에서도 방금 그은 선을 똑같이 제거해야 데이터가 꼬이지 않습니다!
        if (currentConnections.Count > 0)
        {
            currentConnections.RemoveAt(currentConnections.Count - 1);
            Debug.Log("마지막 연결이 취소되었습니다.");
        }
    }

    public void SetCurrentPuzzle(ConstellationData answerData)
    {
        currentAnswerData = answerData;
    }
}