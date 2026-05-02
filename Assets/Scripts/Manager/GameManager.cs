using System;
using System.Threading.Tasks;
using SongLib.Core.Bootstrap;
using SongLib.Core.Singleton;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public int CurrentChapterId = 0;
    public bool isFront = false;

    private Bootstrapper _bootstrapper = new Bootstrapper();

    public async Task EnterNextChapter()
    {
        if (isFront) // 현재 앞부분(true)에서 뒷부분(false)으로 넘어갈 예정이라면
        {
            await StarSpawnManager.Instance.FadeOutAllStars();
        }

        // [데이터 페이즈] 챕터 ID 및 상태 갱신
        if (!isFront) CurrentChapterId++;
        isFront = !isFront;

        ChapterDataRepository.Instance.SetCurrentChapterAddress(CurrentChapterId, isFront);
        await ChapterDataRepository.Instance.LoadCurrentChapterSO();

        // [초기화 페이즈] 부트스트래퍼 공통 세팅
        _bootstrapper = new Bootstrapper();
        Debug.Log($"부트스트래퍼 시작 (Chapter: {CurrentChapterId}, isFront: {isFront})");

        // 공통 매니저 (항상 들어감)
        _bootstrapper
            .Add(DialogueDataRepository.Instance)
            .Add(CutSceneManager.Instance);

        // 챕터 앞부분일 때만 추가되는 매니저
        if (isFront)
        {
            _bootstrapper.Add(StarSpawnManager.Instance);
        }


        // [실행 페이즈] 모두 세팅되었으면 Run!
        _bootstrapper.Run(() => GameStateMachine.Instance.ChangeState<CutSceneState>());

        if (isFront)
        {
            await UITransition.Instance.OpenIris();
        }
    }
}