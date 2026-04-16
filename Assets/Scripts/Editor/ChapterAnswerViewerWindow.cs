using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SongLib; // 원석님의 네임스페이스

public class ChapterAnswerViewerWindow : EditorWindow
{
    private List<ChapterDefinition> _chapterList = new List<ChapterDefinition>();
    private int _currentIndex = 0;
    
    private bool _showAnswer = true;
    private Color _lineColor = Color.cyan;

    // 상단 메뉴에 툴을 등록합니다.
    [MenuItem("StarPath/별자리 정답 뷰어 툴")]
    public static void ShowWindow()
    {
        GetWindow<ChapterAnswerViewerWindow>("정답 뷰어");
    }

    private void OnEnable()
    {
        // 윈도우가 켜질 때, 씬 뷰에 그림을 그리는 이벤트를 구독합니다.
        SceneView.duringSceneGui += OnSceneGUI;
        LoadAllChapters();
    }

    private void OnDisable()
    {
        // 윈도우가 꺼질 때 이벤트를 깔끔하게 해제합니다.
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    // 어드레서블과 상관없이, 에디터 내의 모든 챕터 SO를 긁어옵니다.
    private void LoadAllChapters()
    {
        _chapterList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:ChapterDefinition");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ChapterDefinition chapter = AssetDatabase.LoadAssetAtPath<ChapterDefinition>(path);
            if (chapter != null)
            {
                _chapterList.Add(chapter);
            }
        }
        
        // 이름 순(또는 ID 순)으로 예쁘게 정렬
        _chapterList.Sort((a, b) => string.Compare(a.chapterId, b.chapterId));
    }

    // 에디터 윈도우 창의 UI를 그리는 부분
    private void OnGUI()
    {
        GUILayout.Label("StarPath 별자리 정답 뷰어", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("챕터 데이터 새로고침 (Refresh)"))
        {
            LoadAllChapters();
        }

        if (_chapterList.Count == 0)
        {
            EditorGUILayout.HelpBox("ChapterDefinition SO를 찾을 수 없습니다.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        
        // 챕터 전환 버튼 (이전 / 다음)
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("◀ 이전 챕터", GUILayout.Height(30)))
        {
            _currentIndex = (_currentIndex - 1 + _chapterList.Count) % _chapterList.Count;
            SceneView.RepaintAll(); // 씬 뷰 즉시 새로고침
        }
        
        if (GUILayout.Button("다음 챕터 ▶", GUILayout.Height(30)))
        {
            GameManager.Instance.EnterNextChapter();
            // _currentIndex = (_currentIndex + 1) % _chapterList.Count;
            SceneView.RepaintAll();
        }
        GUILayout.EndHorizontal();

        // 현재 선택된 챕터 정보 표시
        ChapterDefinition currentChapter = _chapterList[_currentIndex];
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"현재 챕터: {currentChapter.chapterId}", MessageType.Info);

        EditorGUILayout.Space();
        
        // 보기 옵션
        _showAnswer = EditorGUILayout.Toggle("정답 선 보여주기", _showAnswer);
        _lineColor = EditorGUILayout.ColorField("선 색상", _lineColor);
    }

    // 씬(Scene) 뷰 화면에 직접 선을 그리는 부분
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!_showAnswer || _chapterList.Count == 0) return;

        ChapterDefinition currentChapter = _chapterList[_currentIndex];
        if (currentChapter.constellationData == null) return;

        Handles.color = _lineColor;

         // ★ 원석님의 ConstellationData 연결 구조에 맞춰서 수정해야 하는 부분
         // 예시: answerConnections라는 리스트가 있다고 가정
         /*
         foreach (var connection in currentChapter.constellationData.answerConnections)
         {
             Vector3 startPos = GetStarPositionInScene(connection.startStarId);
             Vector3 endPos = GetStarPositionInScene(connection.endStarId);

             // 선 두께를 3f로 조금 두껍게 그립니다.
             Handles.DrawLine(startPos, endPos, 3f);
         }
         */
        
        // 그림을 다 그리고 난 뒤 씬 뷰 업데이트 강제
        HandleUtility.Repaint();
    }

    // 씬에 배치된 별 오브젝트를 찾아서 위치를 반환하는 임시 함수
    private Vector3 GetStarPositionInScene(string starId)
    {
        // GameObject.Find(starId) 등 원석님의 씬 구조에 맞게 구현
        return Vector3.zero;
    }
}