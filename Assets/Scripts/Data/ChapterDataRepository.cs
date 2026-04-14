using System;
using System.Collections.Generic;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class ChapterDataRepository : MonoBehaviourSingleton<ChapterDataRepository>, IGameInitializer
{
    public List<ChapterDefinition> chapterDefinitions;
    public Dictionary<string, ChapterDefinition> chapterMap;
    
    public void Initialize(Action onCompleted)
    {
        chapterMap = new Dictionary<string, ChapterDefinition>();
        foreach (var chapterDefinition in chapterDefinitions)
        {
            chapterMap.Add(chapterDefinition.chapterId, chapterDefinition);
        }
        onCompleted?.Invoke();
    }

    public string GetNextChapterKey(string chapterId)
    {
        int index = chapterDefinitions.FindIndex(c => c.chapterId == chapterId);
        
        return index < chapterDefinitions.Count - 1 ? chapterDefinitions[index + 1].chapterId : null;
    }
}
