using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public string ID;
    public string Speaker;
    public string Text_ko;
    public string Text_en;
    public string Text_jp;
}

[System.Serializable]
public class DialogueRoot
{
    public List<DialogueEntry> entries;
}

public class DialogueDatabase : MonoBehaviour
{
    public TextAsset dialogueFile;
    private Dictionary<string, DialogueEntry> map = new();

    void Awake()
    {
        var data = JsonUtility.FromJson<DialogueRoot>(dialogueFile.text);
        foreach (var e in data.entries)
            map[e.ID] = e;
    }

    public DialogueEntry GetLine(string id)
    {
        map.TryGetValue(id, out var entry);
        return entry;
    }
}