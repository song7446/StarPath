using System;
using SongLib;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour, IGameInitializer
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;

    public void Initialize(Action onCompleted)
    {
        _startButton.onClick.AddListener(OnClickStartButton);
        _exitButton.onClick.AddListener(OnClickExitButton);
        
        onCompleted?.Invoke();
    }

    private void OnClickStartButton()
    {
        SceneLoader.Instance.LoadScene(SceneName.GameSceneName);
    }
    
    private void OnClickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}