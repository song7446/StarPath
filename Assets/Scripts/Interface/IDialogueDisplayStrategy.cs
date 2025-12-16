public interface IDialogueDisplayStrategy
{
    /// <summary>
    /// 이 전략으로 대사 표시를 시작한다
    /// </summary>
    void Start(string text);

    /// <summary>
    /// 시간 기반 로직이 필요한 전략을 위한 Tick
    /// 필요 없는 전략은 비워두면 된다
    /// </summary>
    void Tick(float deltaTime);

    /// <summary>
    /// 입력이 들어왔을 때 이 전략이 입력을 소비했는지 여부
    /// true  = 입력을 사용함 (스킵 등)
    /// false = 다음 단계로 넘어가도 됨
    /// </summary>
    bool OnInput();

    /// <summary>
    /// 대사 표시가 완료되었는지 여부
    /// (다음 대사로 갈지는 DialogueManager가 결정)
    /// </summary>
    bool IsFinished { get; }
}