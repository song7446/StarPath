using System.Collections;
using System.Collections.Generic;
using SongLib.Core.Singleton;
using UnityEngine;

public class CutSceneAnimManager : MonoBehaviourSingleton<CutSceneAnimManager>
{
    private Dictionary<string, Animator> _actorAnimators = new();
    
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
}
