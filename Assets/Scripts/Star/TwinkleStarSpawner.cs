using UnityEngine;
using System.Collections.Generic;

public class TwinkleStarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject twinkleStarPrefab;
    [SerializeField] private int starCount = 30;
    [SerializeField] private Vector2 spawnArea = new Vector2(9f, 5f);
    [SerializeField] private float minDistance = 0.7f; // 별 간 최소 거리

    private List<Vector3> spawnedPositions = new List<Vector3>();
    
    public void StartSpawning()
    {
        int attemptLimit = 100; // 무한 루프 방지용
        for (int i = 0; i < starCount; i++)
        {
            Vector3 pos = Vector3.zero;
            bool validPos = false;

            // 일정 횟수 내에서 겹치지 않는 위치 찾기
            for (int attempt = 0; attempt < attemptLimit; attempt++)
            {
                pos = new Vector3(
                    Random.Range(-spawnArea.x, spawnArea.x),
                    Random.Range(-spawnArea.y, spawnArea.y),
                    0f
                );

                if (IsFarEnough(pos))
                {
                    validPos = true;
                    break;
                }
            }

            if (!validPos)
            {
                Debug.LogWarning($"⭐ 별 위치 못 찾음 ({i + 1}/{starCount})");
                continue;
            }

            spawnedPositions.Add(pos);
            Instantiate(twinkleStarPrefab, pos, Quaternion.identity, transform);
        }
    }

    private bool IsFarEnough(Vector3 pos)
    {
        foreach (var existing in spawnedPositions)
        {
            if (Vector3.Distance(pos, existing) < minDistance)
                return false;
        }
        return true;
    }
}