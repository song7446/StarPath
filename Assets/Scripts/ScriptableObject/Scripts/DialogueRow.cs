[System.Serializable]
public class DialogueRow
{
    public string id;
    public string chapterId;
    public string speaker;
    public string textKo;
    public string textEn;
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueRow[] dialogues;
}