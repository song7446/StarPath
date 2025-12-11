using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class DialogueTimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public DialogueManager dialogueManager;

    void Update()
    {
        if (dialogueManager.IsWaiting())
        {
            // 클릭 대기 중이면 Timeline 멈춤
            if (director.state == PlayState.Playing)
                director.Pause();
        }
        else
        {
            // 클릭이 끝났으면 Timeline 다시 재생
            if (director.state == PlayState.Paused)
                director.Resume();
        }
    }
}