using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;

public class ConstellationDataRepository : MonoBehaviourSingleton<ConstellationDataRepository>
{
    private TextAsset _constellationJson;

    private Dictionary<string, ConstellationRow> _constellationMap;
    private List<ConstellationRow> _constellationList;

    private List<DialogueRow> _currentChapterConstellation;
    private int _currentConstellationIdx;
    
    public async Task LoadConstellationData()
    {
        _constellationJson = await AddressableManager.LoadAssetAsync<TextAsset>(ScriptableObjectAddressManager.GetConstellationAddress());
        
        if (_constellationJson == null)
        {
            Debug.LogError("[ConstellationDataRepository] Constellation JSON is missing.");
            return;
        }

        var database = JsonUtility.FromJson<ConstellationDatabase>(_constellationJson.text);
        _constellationMap = new Dictionary<string, ConstellationRow>(database.Constellations.Length);
        _constellationList = new List<ConstellationRow>(database.Constellations.Length);

        foreach (var row in database.Constellations)
        {
            if (_constellationMap.ContainsKey(row.Id))
            {
                Debug.LogWarning($"[DialogueDataRepository] Duplicate dialogue id: {row.Id}");
                continue;
            }

            _constellationMap.Add(row.Id, row);
            _constellationList.Add(row);
        }
        
        AddressableManager.ReleaseAsset(ScriptableObjectAddressManager.GetConstellationAddress());
        
        _constellationJson = null;
        
        Debug.Log("별자리 JSON 원본 메모리 해제 완료 (C# 캐싱 완료)");
    }

    public ConstellationRow GetConstellationRow(string id)
    {
        if (_constellationMap.TryGetValue(id, out var row))
            return row;
        
        Debug.LogWarning($"[ConstellationDataRepository] Constellation not found: {id}");
        return null;
    }
}
