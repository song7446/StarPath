using SongLib.Core.Bootstrap;
using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    private async void Start()
    {
        await DialogueDataRepository.Instance.LoadDialogueJsonSO();
        
        ChapterDataRepository.Instance.SetCurrentChapterAddress(GameManager.Instance.CurrentChapterId, GameManager.Instance.isFront);
        
        await ChapterDataRepository.Instance.LoadCurrentChapterSO();
        
        Bootstrapper _bootstrapper = new Bootstrapper();

        _bootstrapper
            .Add(ChapterDataRepository.Instance)
            .Add(DialogueDataRepository.Instance)
            .Add(CutSceneManager.Instance)
            .Add(StarSpawnManager.Instance)
            .Run(OnBootstrapFinished);
        Destroy(this);
    }
    
    private void OnBootstrapFinished()
    {
        GameStateMachine.Instance.ChangeState<CutSceneState>();
    }
}
