using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class ChapterDataRepository : MonoBehaviourSingleton<ChapterDataRepository>
{
    public ChapterDefinition currentChapterDefinitions;
    
    private string _currentChapterAddress;
    private string _lastLoadedAddress;
    
    public void SetCurrentChapterAddress(int address, bool isFront)
    {
        _currentChapterAddress = ScriptableObjectAddressManager.GetChapterAddress(address, isFront);
    }

    public async Task LoadCurrentChapterSO()
    {
        if (!string.IsNullOrEmpty(_lastLoadedAddress))
        {
            Debug.Log($"이전 챕터({_lastLoadedAddress}) 메모리 해제 중...");
            AddressableManager.ReleaseAsset(_lastLoadedAddress);
            _lastLoadedAddress = null;
        }
        
        Debug.Log($"{_currentChapterAddress} 챕터 데이터 비동기 로딩 시작...");

        // 1. SongLib의 어드레서블 매니저를 통해 SO 데이터를 비동기로 가져옴
        currentChapterDefinitions = await AddressableManager.LoadAssetAsync<ChapterDefinition>(_currentChapterAddress);
        
        // 2. 로딩 성공 시 매니저에 주입
        if (currentChapterDefinitions != null)
        {
            _lastLoadedAddress = _currentChapterAddress;
            
            Debug.Log($"{currentChapterDefinitions.chapterId} 챕터 로딩 성공");
            
            CutSceneManager.Instance.SetCurrentCast(currentChapterDefinitions.cutSceneCasts);
            StarSpawnManager.Instance.SetCurrentConstellation(currentChapterDefinitions.constellationData);
            ConstellationManager.Instance.SetCurrentPuzzle(currentChapterDefinitions.constellationData);
        }
    }
    
    private void OnDestroy()
    {
        AddressableManager.ReleaseAsset(_lastLoadedAddress);
    }
}