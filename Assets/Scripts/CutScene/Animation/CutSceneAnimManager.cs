using System.Collections;
using System.Collections.Generic;
using SongLib.Core.Singleton;
using UnityEngine;

public class CutSceneAnimManager : MonoBehaviourSingleton<CutSceneAnimManager>
{
    private Dictionary<string, Animator> _actorAnimators = new();
    
    private Coroutine _playRoutine;
    
    public void InitAnimators(CutSceneCast cast, List<GameObject> spawnedCharacters)
    {
        _actorAnimators.Clear();

        foreach (var info in cast.characterInfos)
        {
            if (string.IsNullOrEmpty(info.key))
                continue;

            var obj = spawnedCharacters
                .Find(c => c.name == info.characterName);

            if (obj == null)
            {
                Debug.LogWarning($"Spawned 캐릭터 없음: {info.characterName}");
                continue;
            }

            var animator = obj.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"Animator 없음: {info.characterName}");
                continue;
            }

            _actorAnimators[info.key] = animator;
            Debug.Log($"🎭 Animator 등록: {info.key} → {obj.name}");
        }
    }

    public void RegisterAnimator(string actorId, Animator animator)
    {
        _actorAnimators[actorId] = animator;
    }

    public void Clear()
    {
        _actorAnimators.Clear();
    }

    // ================================
    // 🎬 핵심: Play
    // ================================
    public void Play(DialogueAnimationAsset asset)
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
        
        foreach (var cmd in asset.commands)
        {
            if (!_actorAnimators.TryGetValue(cmd.actorId, out var animator))
            {
                Debug.LogWarning($"Animator not found for actorId: {cmd.actorId}");
                continue;
            }

            animator.ResetTrigger(cmd.trigger);
            animator.SetTrigger(cmd.trigger);
        }
        

        // _playRoutine = StartCoroutine(PlayRoutine(asset));
    }

    private IEnumerator PlayRoutine(DialogueAnimationAsset asset)
    {
        bool hasWaitEnd = false;
        List<Animator> waitAnimators = new();

        // 1️⃣ 모든 커맨드 실행
        foreach (var cmd in asset.commands)
        {
            if (!_actorAnimators.TryGetValue(cmd.actorId, out var animator))
            {
                Debug.LogWarning($"Animator not found for actorId: {cmd.actorId}");
                continue;
            }

            animator.ResetTrigger(cmd.trigger);
            animator.SetTrigger(cmd.trigger);

            if (cmd.waitEnd)
            {
                hasWaitEnd = true;
                waitAnimators.Add(animator);
            }
        }

        // 2️⃣ waitEnd가 있는 경우 → 애니메이션 종료 대기
        if (hasWaitEnd)
        {
            foreach (var animator in waitAnimators)
            {
                yield return WaitForAnimationEnd(animator);
            }
        }

        // 3️⃣ DialogueManager에 종료 알림
        DialogueManager.Instance.OnDialogueAnimationFinished();

        _playRoutine = null;
    }

    // ================================
    // 애니메이션 종료 대기
    // ================================
    private IEnumerator WaitForAnimationEnd(Animator animator)
    {
        // 트리거 발동 후 다음 프레임까지 대기
        yield return null;

        // 현재 상태 정보
        var state = animator.GetCurrentAnimatorStateInfo(0);

        // transition 중이면 끝날 때까지
        while (animator.IsInTransition(0))
            yield return null;

        // 애니메이션 끝날 때까지
        while (state.normalizedTime < 1f)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
    }
}
