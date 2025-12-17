using SongLib.Core.Singleton;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [Header("UI")] [SerializeField] private TMP_Text dialogueTextTMP;

    private IDialogueDisplayStrategy _currentStrategy;
    private bool _isDialogueRunning;

    private int _currentDialogueIdx = 0;
    private string _dialogueText;

    private void Update()
    {
        if (!_isDialogueRunning || _currentStrategy == null)
            return;

        _currentStrategy.Tick(Time.deltaTime);
    }

    public void StartDialogue()
    {
        if (DialogueDataRepository.Instance.GetNext(_currentDialogueIdx, out var row))
        {
            _dialogueText = row.textKo;

            // 👉 전략 선택 (지금은 Typewriter 고정)
            _currentStrategy = new TypewriterStrategy(dialogueTextTMP, 0.05f);
            _currentStrategy.Start(_dialogueText);
            _currentDialogueIdx++;

            _isDialogueRunning = true;
        }
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
        
        if (DialogueDataRepository.Instance.GetNext(_currentDialogueIdx, out var row))
        {
            _dialogueText = row.textKo;
            _currentStrategy.Start(_dialogueText);
            _isDialogueRunning = true;
            _currentDialogueIdx++;
        }
        else
        {
            Debug.Log("Dialogue Finished");
        }
    }
}