using System;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public string CurrentChapterId = "Chapter_1_Front";

    public void EnterChapter(string chapterId)
    {
        CurrentChapterId = chapterId;
        Debug.Log($"Enter Chapter: {chapterId}");
    }
}