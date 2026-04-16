using System;
using System.Collections.Generic;
using SongLib;
using SongLib.Core.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarSpawnManager : MonoBehaviourSingleton<StarSpawnManager>, IGameInitializer
{
    [Header("스폰 설정")] public Vector2 spawnAreaSize;
    public float minDistance = 1.5f;
    public int maxSpawnAttempts = 30;

    private GameObject _constellationPrefab;
    public GameObject starPrefab;
    public int fakeStarCount = 10;

    private List<Vector2> occupiedPositions = new List<Vector2>();

    private List<GameObject> _spawnedStars = new List<GameObject>();
    private GameObject _constellationObj;

    public void Initialize(Action onCompleted)
    {
        SpawnStar();
        onCompleted?.Invoke();
    }

    public void SetCurrentConstellation(ConstellationData data)
    {
        if (data == null)
        {
            Debug.LogError("전달받은 ConstellationData가 Null입니다! SO 세팅을 확인하세요.");
            return;
        }

        _constellationPrefab = data.constellationPrefab;
    }

    public void SpawnStar()
    {
        occupiedPositions.Clear();

        Vector2 constellationCenter = new Vector2(
            transform.position.x + Random.Range(-spawnAreaSize.x / 6f, spawnAreaSize.x / 6f),
            transform.position.y + Random.Range(-spawnAreaSize.y / 6f, spawnAreaSize.y / 6f)
        );

        _constellationObj =
            Instantiate(_constellationPrefab, constellationCenter, Quaternion.identity, transform);

        foreach (Transform child in _constellationObj.transform)
        {
            PuzzleStar star = child.GetComponent<PuzzleStar>();
            if (star != null)
            {
                star.SetupStar(occupiedPositions.Count + 1, true);
                occupiedPositions.Add(child.position);
            }
        }

        // 2. 방해 별 스폰
        for (int i = 0; i < fakeStarCount; i++)
        {
            Vector2? validPos = GetValidRandomPos();

            if (validPos.HasValue)
            {
                GameObject starObj = Instantiate(starPrefab, validPos.Value, Quaternion.identity, transform);
                _spawnedStars.Add(starObj);
                PuzzleStar star = starObj.GetComponent<PuzzleStar>();
                star.SetupStar(occupiedPositions.Count + 1, true);
                occupiedPositions.Add(validPos.Value);
            }
            else
            {
                Debug.LogWarning($"공간이 부족하여 {i + 1}번째 방해 별을 배치하지 못했습니다.");
            }
        }
    }

    private Vector2? GetValidRandomPos()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomPos = new Vector2(
                transform.position.x + Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                transform.position.y + Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
            );

            if (IsPositionSafe(randomPos))
            {
                return randomPos;
            }
        }

        return null;
    }

    private bool IsPositionSafe(Vector2 pos)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(pos, occupiedPos) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}