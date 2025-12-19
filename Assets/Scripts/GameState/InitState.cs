using SongLib.Core.Bootstrap;
using SongLib.Patterns.State;

public class InitState : IState
{
    public void OnEnter()
    {
        Bootstrapper _bootstrapper = new Bootstrapper();

        _bootstrapper
            .Add(DialogueDataRepository.Instance)
            .Run(OnBootstrapFinished);
    }

    private void OnBootstrapFinished()
    {
        GameStateMachine.Instance.ChangeState<CutSceneState>();
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnExit()
    {
    }
}