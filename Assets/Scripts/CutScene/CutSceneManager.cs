using System;
using System.Collections.Generic;
using SongLib.Core.Singleton;
using UnityEngine;

public class CutSceneManager : MonoBehaviourSingleton<CutSceneManager>
{
    public List<GameObject> _spawnedCharacters = new();
    public List<GameObject> _spawnedObjects = new();

    [SerializeField] private CutSceneCast[] _casts;

    private void Start()
    {
        InitCutScene(_casts[0]);
    }

    public void InitCutScene(CutSceneCast cast)
    {
        foreach (var info in cast.characterInfos)
        {
            if (info.characterPrefab == null) continue;

            var obj = Instantiate(info.characterPrefab, info.position, Quaternion.Euler(info.rotation));
            obj.name = info.characterName;
            _spawnedCharacters.Add(obj);
        }

        // 2️⃣ 오브젝트 생성
        foreach (var info in cast.objectInfos)
        {
            if (info.objectPrefab == null) continue;

            var obj = Instantiate(info.objectPrefab, info.position, Quaternion.Euler(info.rotation));
            obj.name = info.objectName;
            _spawnedObjects.Add(obj);
        }

        if (cast.timelineAsset == null)
        {
            CutSceneAnimManager.Instance.InitAnimators(cast, _spawnedCharacters);
            DialogueManager.Instance.StartDialogue();
        }
        else
        {
            TimelineManager.Instance.PlayCutscene(cast);
        }
    }
}