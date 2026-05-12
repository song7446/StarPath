using SongLib.Core.Bootstrap;
using UnityEngine;

public class TitleSceneEntryPoint : MonoBehaviour
{
    private void Start()
    {
        Bootstrapper _bootstrapper = new Bootstrapper();

        _bootstrapper
            .Add(GameStateMachine.Instance)
            .Add(TitleSceneUIManager.Instance.titleUI)
            .Run(OnBootstrapFinished);
        Destroy(this);
    }
    
    private void OnBootstrapFinished()
    {
        GameStateMachine.Instance.ChangeState<TitleState>();
    }
}
