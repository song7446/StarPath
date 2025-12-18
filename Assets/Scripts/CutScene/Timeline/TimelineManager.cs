using System.Collections.Generic;
using SongLib.Core.Singleton;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class TimelineManager : MonoBehaviourSingleton<TimelineManager>
{
    private bool _isPlaying = false;
    [SerializeField] private PlayableDirector _director;
    private readonly List<GameObject> _spawnedCharacters = new();
    private readonly List<GameObject> _spawnedObjects = new();
    
    [SerializeField] private SignalReceiver _signalReceiver;
    public bool IsWaitingForInput;

    public void PlayCutscene(CutSceneCast cast)
    {
        if (_isPlaying)
        {
            Debug.LogWarning("Cutscene already playing!");
            return;
        }

        _isPlaying = true;
        Debug.Log($"🎬 Cutscene Start: {cast.name}");
        

        if (cast.timelineAsset != null)
        {
            // 3️⃣ PlayableDirector 생성 및 설정
            _director.playableAsset = cast.timelineAsset;

            // 4️⃣ Track 자동 바인딩
            AutoBindTracks(_director, cast);

            // 5️⃣ 컷씬 재생
            _director.Play();
            // _director.stopped += OnCutsceneEnd;
        }
    }

    private void AutoBindTracks(PlayableDirector director, CutSceneCast cast)
    {
        if (!(director.playableAsset is TimelineAsset timeline)) 
            return;

        // 🔹 key → GameObject 매핑 딕셔너리 구성
        Dictionary<string, GameObject> keyToObject = new();

        // 캐릭터 등록
        foreach (var info in cast.characterInfos)
        {
            if (info.characterPrefab == null) continue;

            // 이미 씬에 생성된 캐릭터를 찾는다 (PlayCutscene 단계에서 Instantiate 후 등록해둔 경우)
            var obj = GameObject.Find(info.characterName);
            if (obj != null)
                keyToObject[info.key] = obj;
        }

        // 오브젝트 등록
        foreach (var info in cast.objectInfos)
        {
            if (info.objectPrefab == null) continue;

            var obj = GameObject.Find(info.objectName);
            if (obj != null)
                keyToObject[info.key] = obj;
        }

        // 🔹 Timeline 트랙 순회하면서 key 기반으로 자동 바인딩
        foreach (var track in timeline.GetOutputTracks())
        {
            string trackKey = track.name;
            
            if (track is SignalTrack)
            {
                if (_signalReceiver != null)
                {
                    director.SetGenericBinding(track, _signalReceiver);
                    Debug.Log($"SignalReceiver bound to track: {track.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ No SignalReceiver found on {name}. Add one to handle signals.");
                }
                continue;
            }

            if (keyToObject.TryGetValue(trackKey, out var target))
            {
                var animator = target.GetComponent<Animator>();
                director.SetGenericBinding(track, animator != null ? animator as Object : target);
            }
            else
            {
                Debug.LogWarning($"⚠️ No binding found for track key: {trackKey}");
            }
        }
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
        Debug.Log("🎬 Cutscene Ended.");
        _isPlaying = false;

        // 생성된 캐릭터와 오브젝트 정리
        foreach (var c in _spawnedCharacters)
            if (c != null) Destroy(c);
        _spawnedCharacters.Clear();

        foreach (var o in _spawnedObjects)
            if (o != null) Destroy(o);
        _spawnedObjects.Clear();

        // PlayableDirector 제거
        if (_director != null)
            Destroy(_director.gameObject);
    }

    public void WaitForInput()
    {
        if (_director == null)
            return;

        _director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        IsWaitingForInput = true;
        Debug.Log("⏸ 컷씬 일시정지 - 입력 대기 중");
    }

    public void ContinueCutscene()
    {
        if (_director == null)
        {
            Debug.LogWarning("❗ Director not found for ContinueCutscene");
            return;
        }

        if (!IsWaitingForInput)
        {
            Debug.Log("컷씬 입력 대기 상태가 아님");
            return;
        }

        IsWaitingForInput = false;
        _director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        Debug.Log("▶ 컷씬 재개");
    }
}
