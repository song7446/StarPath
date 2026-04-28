using SongLib.Core.Singleton;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [Header("UI")] [SerializeField] private TMP_Text dialogueTextTMP;

    private IDialogueDisplayStrategy _currentStrategy;
    private bool _isDialogueRunning;

    private string _dialogueText;

    private void Update()
    {
        if (!_isDialogueRunning || _currentStrategy == null)
            return;

        _currentStrategy.Tick(Time.deltaTime);
    }

    public void StartDialogue()
    {
        if (DialogueDataRepository.Instance.GetNextDialogue(out var row))
        {
            _dialogueText = row.textKo;

            TryPlayDialogueAnimation(row.id);

            // 👉 전략 선택 (지금은 Typewriter 고정)
            _currentStrategy = new TypewriterStrategy(dialogueTextTMP, 0.05f);
            _currentStrategy.Start(_dialogueText);

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

        if (DialogueDataRepository.Instance.GetNextDialogue(out var row))
        {
            _dialogueText = row.textKo;
            TryPlayDialogueAnimation(row.id);
            _currentStrategy.Start(_dialogueText);
            _isDialogueRunning = true;
        }
        else
        {
            Debug.Log("Dialogue Finished");

            if (GameManager.Instance.isFront)
            {
                GameStateMachine.Instance.ChangeState<GamePlayState>();
            }
            else
            {
                UITransition.Instance.CloseIris();
                
                GameManager.Instance.EnterNextChapter();
            }
        }
    }

    private void TryPlayDialogueAnimation(string dialogueId)
    {
        var repo = ChapterDataRepository.Instance;
        if (repo == null || repo.currentChapterDefinitions == null)
            return;


        DialogueAnimationAsset[] animAssets = repo.currentChapterDefinitions.dialogueAnimations;
        foreach (var animAsset in animAssets)
        {
            if (animAsset.dialogueId == dialogueId)
            {
                CutSceneAnimManager.Instance.Play(animAsset);
                return;
            }
        }
    }
}