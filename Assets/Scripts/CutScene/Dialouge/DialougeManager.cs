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

    private bool _isWaitingForAnimation;

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

            TryPlayDialogueAnimation(row.id);

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
            TryPlayDialogueAnimation(row.id);
            _currentStrategy.Start(_dialogueText);
            _isDialogueRunning = true;
            _currentDialogueIdx++;
        }
        else
        {
            Debug.Log("Dialogue Finished");
        }
    }

    private void TryPlayDialogueAnimation(string dialogueId)
    {
        var repo = DialogueDataRepository.Instance;
        if (repo == null || repo.chapterDefinitions == null)
            return;

        // 👉 chapterId는 상위 컨텍스트에서 가져온다
        var chapterId = GameManager.Instance.CurrentChapterId;

        DialogueAnimationAsset[] animAssets = repo.chapterMap[chapterId].dialogueAnimations;
        foreach (var animAsset in animAssets)
        {
            if (animAsset.dialogueId == dialogueId)
            {
                CutSceneAnimManager.Instance.Play(animAsset);
                
                if (HasWaitEndCommand(animAsset))
                {
                    _isWaitingForAnimation = true;
                    _isDialogueRunning = false;
                }
                return;
            }
        }
    }

    private bool HasWaitEndCommand(DialogueAnimationAsset asset)
    {
        foreach (var cmd in asset.commands)
        {
            if (cmd.waitEnd)
                return true;
        }

        return false;
    }

    public void OnDialogueAnimationFinished()
    {
        _isWaitingForAnimation = false;
        OnDialogueFinished();
    }
}