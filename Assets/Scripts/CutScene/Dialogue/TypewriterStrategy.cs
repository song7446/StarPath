using UnityEngine;
using TMPro;

public class TypewriterStrategy : IDialogueDisplayStrategy
{
    private readonly TMP_Text _textUI;
    private readonly float _charInterval;

    private string _fullText;
    private int _currentIndex;
    private float _timer;
    private bool _isFinished;

    public bool IsFinished => _isFinished;

    public TypewriterStrategy(TMP_Text textUI, float charInterval = 0.05f)
    {
        _textUI = textUI;
        _charInterval = charInterval;
    }

    public void Start(string text)
    {
        _fullText = text;
        _currentIndex = 0;
        _timer = 0f;
        _isFinished = false;

        _textUI.text = string.Empty;
    }

    public void Tick(float deltaTime)
    {
        if (_isFinished)
            return;

        _timer += deltaTime;

        while (_timer >= _charInterval)
        {
            _timer -= _charInterval;
            RevealNextChar();
        }
    }

    public bool OnInput()
    {
        // 타이핑 중이면 → 즉시 전부 출력 (입력 소비)
        if (!_isFinished)
        {
            CompleteInstantly();
            return true;
        }

        // 이미 완료된 상태면 → 입력 소비 안 함
        return false;
    }

    private void RevealNextChar()
    {
        if (_currentIndex >= _fullText.Length)
        {
            _isFinished = true;
            return;
        }

        _currentIndex++;
        _textUI.text = _fullText.Substring(0, _currentIndex);

        if (_currentIndex >= _fullText.Length)
        {
            _isFinished = true;
        }
    }

    private void CompleteInstantly()
    {
        _textUI.text = _fullText;
        _currentIndex = _fullText.Length;
        _isFinished = true;
    }
}