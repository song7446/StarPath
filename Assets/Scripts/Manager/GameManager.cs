using System;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] public int CurrentChapterId = 1;
    [SerializeField] public bool isFront = true;
}