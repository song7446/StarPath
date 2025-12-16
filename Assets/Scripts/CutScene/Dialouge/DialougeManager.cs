using SongLib.Core.Singleton;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [Header("UI")]
    [SerializeField] private TMP_Text dialogueText;

    private IDialogueDisplayStrategy _currentStrategy;
    private bool _isDialogueRunning;

    private void Update()
    {
        if (!_isDialogueRunning || _currentStrategy == null)
            return;

        _currentStrategy.Tick(Time.deltaTime);
    }

    public void StartDialogue(string text)
    {
        // 👉 전략 선택 (지금은 Typewriter 고정)
        _currentStrategy = new TypewriterStrategy(dialogueText, 0.05f);
        _currentStrategy.Start(text);

        _isDialogueRunning = true;
    }

    public void OnInput()
    {
        if (!_isDialogueRunning || _currentStrategy == null)
            return;

        bool consumed = _currentStrategy.OnInput();

        // 전략이 입력을 소비하지 않았고, 표시가 끝났다면
        if (!consumed && _currentStrategy.IsFinished)
        {
            OnDialogueFinished();
        }
    }

    private void OnDialogueFinished()
    {
        _isDialogueRunning = false;

        // 여기서 다음 행동을 결정
        // 1. 다음 대사
        // 2. Timeline Resume
        // 3. 컷씬 종료

        Debug.Log("Dialogue Finished");
    }
}