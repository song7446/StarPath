using System;
using SongLib;
using SongLib.Core.Singleton;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviourSingleton<GameSceneUIManager>, IGameInitializer
{
    [Header("가상 카메라 (Main Camera 제어용)")]
    [SerializeField] private CinemachineCamera ccGround;
    [SerializeField] private CinemachineCamera ccSky;
    [SerializeField] private CinemachineCamera ccStarSky;
    [SerializeField] private CinemachineCamera ccStarGround;
    
    public Camera StarCamera; 
    
    [SerializeField] private Button _lookUpCameraButton;
    
    public bool IsSkyView = false;
    
    [SerializeField] private ConstellationUI _constellationUI;
    
    [SerializeField] private NoteBookUI _notebookUI;
    [SerializeField] private Button _showNotebookButton;
    [SerializeField] private Button _hideGuideImageButton;

    public void Initialize(Action onCompleted)
    {
        ccGround.Priority = 10;
        ccSky.Priority = 0;

        ccStarGround.Priority = 10;
        ccStarSky.Priority = 0;
        
        _lookUpCameraButton.onClick.AddListener(OnClickLookUpCameraButton);
        
        _showNotebookButton.onClick.AddListener(OnClickShowNotebookButton);
        
        _hideGuideImageButton.onClick.AddListener(OnClickHideGuideImageButton);
        
        _notebookUI.SetupNotebookData();
        
        onCompleted?.Invoke();
    }

    /// <summary>
    /// UI 버튼 하나로 하늘 뷰와 땅 뷰를 토글(왕복)합니다.
    /// </summary>
    public void OnClickLookUpCameraButton()
    {
        // 💡 버튼을 누를 때마다 상태를 반대로 뒤집음 (true -> false, false -> true)
        IsSkyView = !IsSkyView; 

        if (IsSkyView)
        {
            // [하늘 뷰로 올라갈 때]
            ccSky.Priority = 20;
            ccStarSky.Priority = 20;
            Debug.Log("카메라 이동: 땅 -> 하늘 (캐릭터 숨김)");
        }
        else
        {
            // [땅 뷰로 내려올 때]
            ccSky.Priority = 0;
            ccStarSky.Priority = 0;
            Debug.Log("카메라 이동: 하늘 -> 땅 (캐릭터 표시)");
        }
    }
    
    private void OnClickShowNotebookButton()
    {
        if (_notebookUI != null)
        {
            _notebookUI.ToggleNotebook();
        }
    }
    
    private void OnClickHideGuideImageButton()
    {
        _constellationUI.ToggleGuideImage();
    }
    
    public void UpdateGuide(ConstellationData constellationData)
    {
        if (_constellationUI != null)
        {
            _constellationUI.UpdateGuide(constellationData);
        }
        else
        {
            Debug.LogWarning("ConstellationUI가 연결되지 않았습니다!");
        }
    }
}
