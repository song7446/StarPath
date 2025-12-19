using SongLib.Core.Bootstrap;
using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    private void Start()
    {
        Bootstrapper _bootstrapper = new Bootstrapper();

        _bootstrapper
            .Add(DialogueDataRepository.Instance)
            .Add(CutSceneManager.Instance)
            .Run(OnBootstrapFinished);
        Destroy(this);
    }
    
    private void OnBootstrapFinished()
    {
        GameStateMachine.Instance.ChangeState<CutSceneState>();
    }
}
