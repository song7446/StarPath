using System;
using System.Collections.Generic;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class CutSceneManager : MonoBehaviourSingleton<CutSceneManager>,IGameInitializer
{
    public List<GameObject> _spawnedCharacters = new();
    public List<GameObject> _spawnedObjects = new();
    
    public CutSceneCast CurrentCast;

    public void Initialize(Action onCompleted)
    {
        foreach (var info in CurrentCast.characterInfos)
        {
            if (info.characterPrefab == null) continue;

            var obj = Instantiate(info.characterPrefab, info.position, Quaternion.Euler(info.rotation));
            obj.name = info.characterName;
            _spawnedCharacters.Add(obj);
        }

        // 2️⃣ 오브젝트 생성
        foreach (var info in CurrentCast.objectInfos)
        {
            if (info.objectPrefab == null) continue;

            var obj = Instantiate(info.objectPrefab, info.position, Quaternion.Euler(info.rotation));
            obj.name = info.objectName;
            _spawnedObjects.Add(obj);
        }

        StartCutScene();
        
        onCompleted?.Invoke();
    }
    
    public void SetCurrentCast(CutSceneCast cast)
    {
        CurrentCast = cast;
    }

    public void StartCutScene()
    {
        if (CurrentCast.timelineAsset == null)
        {
            CutSceneAnimManager.Instance.InitAnimators(CurrentCast, _spawnedCharacters);
            DialogueManager.Instance.StartDialogue();
        }
        else
        {
            TimelineManager.Instance.PlayCutscene(CurrentCast);
        }
    }
}