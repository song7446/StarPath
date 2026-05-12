using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class DialogueDataRepository : MonoBehaviourSingleton<DialogueDataRepository>, IGameInitializer
{
    private TextAsset _dialogueJson;

    private Dictionary<string, DialogueRow> _dialogueMap;
    private List<DialogueRow> _dialogueList;

    private List<DialogueRow> _currentChapterDialogues;
    private int _currentDialogueIdx;
    
    public async Task LoadDialogueJsonSO()
    {
        _dialogueJson = await AddressableManager.LoadAssetAsync<TextAsset>(ScriptableObjectAddressManager.GetDialogueJsonAddress());
        
        if (_dialogueJson == null)
        {
            Debug.LogError("[DialogueDataRepository] Dialogue JSON is missing.");
            return;
        }

        var database = JsonUtility.FromJson<DialogueDatabase>(_dialogueJson.text);
        _dialogueMap = new Dictionary<string, DialogueRow>(database.Dialogues.Length);
        _dialogueList = new List<DialogueRow>(database.Dialogues.Length);

        foreach (var row in database.Dialogues)
        {
            if (_dialogueMap.ContainsKey(row.Id))
            {
                Debug.LogWarning($"[DialogueDataRepository] Duplicate dialogue id: {row.Id}");
                continue;
            }

            _dialogueMap.Add(row.Id, row);
            _dialogueList.Add(row);
        }
        
        AddressableManager.ReleaseAsset(ScriptableObjectAddressManager.GetDialogueJsonAddress());
        
        _dialogueJson = null;
        
        Debug.Log("대사 JSON 원본 메모리 해제 완료 (C# 캐싱 완료)");
    }
    
    public void Initialize(Action onCompleted)
    {
        // ★ 방어 코드: 비동기 로딩 단계에서 리스트가 안 만들어졌다면?
        if (_dialogueList == null || _dialogueList.Count == 0)
        {
            Debug.LogError("[DialogueDataRepository] 대사 리스트가 비어있습니다. LoadDialogueJsonSO가 실패했는지 확인하세요.");
            
            // 데이터가 없어도 부트스트래퍼가 다음 순서로 넘어가도록 완료 보고는 해줍니다.
            onCompleted?.Invoke();
            return;
        }

        // 안전하게 챕터 대사 필터링
        SetDialogue(ChapterDataRepository.Instance.currentChapterDefinitions.chapterId);
        
        onCompleted?.Invoke();
    }
    
    public void SetDialogue(string chapterId)
    {
        var repo = ChapterDataRepository.Instance;
        
        if (repo.currentChapterDefinitions == null)
        {
            Debug.LogError($"[DialogueDataRepository] Chapter definitions not found for: {chapterId}");
            return;
        }

        _currentChapterDialogues = new List<DialogueRow>();
        _currentDialogueIdx = 0;

        bool isMyChapter = false;
        
        foreach (var row in _dialogueList)
        {
            if (row.ChapterId == chapterId)
            {
                _currentChapterDialogues.Add(row);
                isMyChapter = true;
            }
            else if (isMyChapter)
            {
                break;
            }
        }

        if (_currentChapterDialogues.Count == 0)
        {
            Debug.LogWarning($"[DialogueDataRepository] No dialogues found for chapter: {chapterId}");
        }
    }
    
    public DialogueRow GetDialogueRow(string id)
    {
        if (_dialogueMap.TryGetValue(id, out var row))
            return row;

        Debug.LogWarning($"[DialogueDataRepository] Dialogue not found: {id}");
        return null;
    }

    public bool Contains(string id)
    {
        return _dialogueMap.ContainsKey(id);
    }

    public bool GetNextDialogue(out DialogueRow row)
    {
        if (_currentChapterDialogues == null ||
            _currentDialogueIdx >= _currentChapterDialogues.Count)
        {
            row = null;
            return false;
        }

        row = _currentChapterDialogues[_currentDialogueIdx];
        _currentDialogueIdx++;
        return true;
    }
}