public static class ScriptableObjectAddressManager
{
    public static string GetChapterAddress(int chapterId, bool isFront)
    {
        return isFront ? $"Chapter_{chapterId}_Front" : $"Chapter_{chapterId}_Back";
    }
    public static string GetChapterPuzzleAddress(int chapterId)
    {
        return $"Chapter_{chapterId}_Puzzle";
    }
    
    public static string GetDialogueJsonAddress()
    {
        return "dialogue";
    }
}
