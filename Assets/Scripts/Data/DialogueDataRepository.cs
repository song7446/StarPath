using System;
using System.Collections.Generic;
using System.Linq;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class DialogueDataRepository : MonoBehaviourSingleton<DialogueDataRepository>, IGameInitializer
{
    [SerializeField] private TextAsset dialogueJson;

    private Dictionary<string, DialogueRow> _dialogueMap;
    private List<DialogueRow> _dialogueList;

    private List<DialogueRow> _currentChapterDialogues;
    private int _currentDialogueIdx;
    
    public void Initialize(Action onCompleted)
    {
        if (dialogueJson == null)
        {
            Debug.LogError("[DialogueDataRepository] Dialogue JSON is missing.");
            return;
        }

        var database = JsonUtility.FromJson<DialogueDatabase>(dialogueJson.text);
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

        SetDialogue(GameManager.Instance.CurrentChapterId);
        
        onCompleted?.Invoke();
    }
    
    public void SetDialogue(string chapterId)
    {
        var repo = ChapterDataRepository.Instance;
        if (!repo.chapterMap.TryGetValue(chapterId, out var chapter))
        {
            Debug.LogError($"Chapter not found: {chapterId}");
            return;
        }

        _currentChapterDialogues = new List<DialogueRow>();
        _currentDialogueIdx = 0;

        foreach (var row in _dialogueList)
        {
            if (row.chapterId == chapterId)
            {
                _currentChapterDialogues.Add(row);
            }
            else
            {
                break;
            }
        }
        
        CutSceneManager.Instance.SetCurrentCast(repo.chapterMap[chapterId].cutSceneCasts);

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