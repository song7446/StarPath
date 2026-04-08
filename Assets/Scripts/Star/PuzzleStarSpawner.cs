using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleStarSpawner : MonoBehaviour
{
    [Header("스폰 설정")] public Vector2 spawnAreaSize;
    public float minDistance = 1.5f;
    public int maxSpawnAttempts = 30;

    public ConstellationData currentData;
    public GameObject constellationPrefab;
    public GameObject starPrefab;
    public int fakeStarCount = 10;

    private List<Vector2> occupiedPositions = new List<Vector2>();

    private void Start()
    {
        SpawnStar();
    }

    public void SpawnStar()
    {
        occupiedPositions.Clear();

        // [수정 포인트 2] 정답 별자리 프리팹의 크기를 고려해서 중심점 배치 범위를 조금 더 좁게(/4f -> /6f 등) 설정하거나 여백(margin)을 줍니다.
        // 프리팹이 클수록 이 범위를 좁혀야 기즈모 밖으로 튀어나가지 않습니다.
        Vector2 constellationCenter = new Vector2(
            transform.position.x + Random.Range(-spawnAreaSize.x / 6f, spawnAreaSize.x / 6f),
            transform.position.y + Random.Range(-spawnAreaSize.y / 6f, spawnAreaSize.y / 6f)
        );

        GameObject constellationObj =
            Instantiate(constellationPrefab, constellationCenter, Quaternion.identity, transform);

        foreach (Transform child in constellationObj.transform)
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