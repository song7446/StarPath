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
    }
    
    public void Initialize(Action onCompleted)
    {
        if (_dialogueJson == null)
        {
            Debug.LogError("[DialogueDataRepository] Dialogue JSON is missing.");
            return;
        }

        var database = JsonUtility.FromJson<DialogueDatabase>(_dialogueJson.text);
        _dialogueMap = new Dictionary<string, DialogueRow>(database.dialogues.Length);
        _dialogueList = new List<DialogueRow>(database.dialogues.Length);

        foreach (var row in database.dialogues)
        {
            if (_dialogueMap.ContainsKey(row.id))
            {
                Debug.LogWarning($"[DialogueDataRepository] Duplicate dialogue id: {row.id}");
                continue;
            }

            _dialogueMap.Add(row.id, row);
            _dialogueList.Add(row);
        }

        SetDialogue(ChapterDataRepository.Instance.currentChapterDefinitions.chapterId);
        
        AddressableManager.ReleaseAsset(ScriptableObjectAddressManager.GetDialogueJsonAddress());
        _dialogueJson = null; // 혹시 모를 참조를 막기 위해 null 처리
        Debug.Log("대사 JSON 원본 메모리 해제 완료 (C# 캐싱 완료)");
        
        onCompleted?.Invoke();
    }
    
    public void SetDialogue(string chapterId)
    {
        var repo = ChapterDataRepository.Instance;
        
        if (repo.currentChapterDefinitions == null)
        {
            Debug.LogError($"Chapter not found: {chapterId}");
            return;
        }

        _currentChapterDialogues = new List<DialogueRow>();
        _currentDialogueIdx = 0;

        bool isMyChapter = false;

        foreach (var row in _dialogueList)
        {
            if (row.chapterId == chapterId)
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
            Debug.LogWarning($"No dialogues for chapter: {chapterId}");
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