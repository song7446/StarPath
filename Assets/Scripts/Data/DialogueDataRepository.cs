using System.Collections.Generic;
using SongLib.Core.Singleton;
using UnityEngine;

public class DialogueDataRepository : MonoBehaviourSingleton<DialogueDataRepository>
{
    [SerializeField] private TextAsset dialogueJson;

    private Dictionary<string, DialogueRow> _dialogueMap;
    private List<DialogueRow> _dialogueList;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
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
    }

    public DialogueRow Get(string id)
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

    public bool GetNext(int currentIDx, out DialogueRow row)
    {
        if (currentIDx < _dialogueList.Count - 1)
            row = _dialogueList[currentIDx + 1];
        else
            row = null;
        
        return currentIDx < _dialogueList.Count - 1;
    }
}