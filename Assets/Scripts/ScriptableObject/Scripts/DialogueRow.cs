[System.Serializable]
public class DialogueRow
{
    public string Id;
    public string ChapterId;
    public string Speaker;
    public string TextKo;
    public string TextEn;
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueRow[] Dialogues;
}