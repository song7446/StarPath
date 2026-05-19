using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ConstellationDataRepository : MonoBehaviourSingleton<ConstellationDataRepository>
{
    private TextAsset _constellationJson;

    private Dictionary<string, ConstellationRow> _constellationRowMap;
    private Dictionary<string, ConstellationData> _constellationDataMap;
    private List<ConstellationRow> _allConstellationsRow;
    private List<ConstellationData> _allConstellationsData;
    
    private List<string> _orderedConstellationIds;
    
    private int _currentConstellationIdx;

    public async Task LoadConstellationData()
    {
        // 1. JSON 텍스트 로드 및 캐싱 (기존 로직과 동일)
        var jsonText = await AddressableManager.LoadAssetAsync<TextAsset>(ScriptableObjectAddressManager.GetConstellationAddress());
        var database = JsonUtility.FromJson<ConstellationDatabase>(jsonText.text);
        
        _constellationRowMap = new Dictionary<string, ConstellationRow>();
        _orderedConstellationIds = new List<string>();
        _allConstellationsRow =  new List<ConstellationRow>();
        _allConstellationsData = new List<ConstellationData>();

        foreach (var row in database.Constellations)
        {
            if (!_constellationRowMap.ContainsKey(row.Id))
            {
                _constellationRowMap.Add(row.Id, row);
                _allConstellationsRow.Add(row);
                _orderedConstellationIds.Add(row.Id); // 들어온 순서(챕터 순서)대로 ID 저장
            }
        }
        AddressableManager.ReleaseAsset(ScriptableObjectAddressManager.GetConstellationAddress());

        // 2. SO 에셋 다중 로드 (라벨 사용)
        _constellationDataMap = new Dictionary<string, ConstellationData>();
        
        // "Constellation" 이라는 라벨이 붙은 모든 SO를 비동기로 로드
        var handle = Addressables.LoadAssetsAsync<ConstellationData>("Constellation", asset =>
        {
            // 각 에셋이 로드될 때마다 콜백 실행
            // SO 내부에 Id (또는 이름) 프로퍼티가 있다고 가정
            if (asset != null && !_constellationDataMap.ContainsKey(asset.constellationId)) 
            {
                _constellationDataMap.Add(asset.constellationId, asset);
                _allConstellationsData.Add(asset);
            }
        });

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"별자리 에셋 로드 완료: {_constellationDataMap.Count}개");
        }
        else
        {
            Debug.LogError("별자리 에셋 로드 실패");
        }
    }

    public ConstellationRow GetConstellationRow(string id)
    {
        if (_constellationRowMap.TryGetValue(id, out var row))
            return row;
        
        Debug.LogWarning($"[ConstellationDataRepository] Constellation not found: {id}");
        return null;
    }

    public List<ConstellationDisplayData> GetUnlockedData()
    {
        List<ConstellationDisplayData> unlockedList = new List<ConstellationDisplayData>();
        
        // 챕터 순서가 보장된 ID 리스트(_orderedConstellationIds)를 기준으로 돕니다!
        int maxIndex = Mathf.Min(GameManager.Instance.CurrentChapterId, _orderedConstellationIds.Count);
        
        for (int i = 0; i < maxIndex; i++)
        {
            string targetId = _orderedConstellationIds[i];

            // ID를 키값으로 딕셔너리에서 정확한 짝을 찾아옵니다.
            if (_constellationRowMap.TryGetValue(targetId, out var row) && 
                _constellationDataMap.TryGetValue(targetId, out var data))
            {
                unlockedList.Add(new ConstellationDisplayData
                {
                    Row = row,
                    Data = data
                });
            }
        }
    
        return unlockedList;
    }
}
