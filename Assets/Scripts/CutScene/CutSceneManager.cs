using SongLib.Core.Singleton;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutSceneManager : MonoBehaviourSingleton<CutSceneManager>
{
    public PlayableDirector director;

    public void PlayCutScene(TimelineAsset timelineAsset)
    {
        if (timelineAsset == null)
        {
            Debug.LogError("timelineAsset is null");
            return;
        }
        
        director.playableAsset = timelineAsset;
        director.time = 0;
        director.Play(timelineAsset);
        
        Debug.Log($"{timelineAsset.name} is played");
    }
}
