using System;
using System.Threading.Tasks;
using SongLib.Core.Bootstrap;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private int CurrentChapterId = 0;
    private bool isFront = false;
    
    Bootstrapper _bootstrapper = new Bootstrapper();
    
    
    public async Task EnterNextChapter()
    {
        if (!isFront)
        {
            CurrentChapterId++;
        }
        isFront = !isFront;
        
        ChapterDataRepository.Instance.SetCurrentChapterAddress(CurrentChapterId, isFront);
        
        await ChapterDataRepository.Instance.LoadCurrentChapterSO();

        _bootstrapper
            .Add(DialogueDataRepository.Instance)
            .Add(CutSceneManager.Instance)
            .Add(StarSpawnManager.Instance)
            .Run(()=>GameStateMachine.Instance.ChangeState<CutSceneState>());
    }
}