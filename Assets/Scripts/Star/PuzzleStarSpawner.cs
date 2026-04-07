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
                star.SetupStar(occupiedPositions.Count,true);
                occupiedPositions.Add(child.position);
            }
        }

        // 2. 방해 별 스폰
        for (int i = 0; i < fakeStarCount; i++)
        {
            // [수정 포인트 1-A] Nullable(Vector2?)을 사용하여 실패 여부를 확실히 받음
            Vector2? validPos = GetValidRandomPos();

            if (validPos.HasValue) // 값을 찾은 경우에만 스폰!
            {
                // validPos.Value로 실제 Vector2 값을 꺼내서 씁니다.
                GameObject starObj= Instantiate(starPrefab, validPos.Value, Quaternion.identity, transform);
                PuzzleStar star = starObj.GetComponent<PuzzleStar>();
                star.SetupStar(occupiedPositions.Count,true);
                occupiedPositions.Add(validPos.Value);
            }
            else
            {
                // 실패했다면 (0,0)에 스폰하지 않고 그냥 이 별은 스폰을 포기(또는 경고만)
                Debug.LogWarning($"공간이 부족하여 {i + 1}번째 방해 별을 배치하지 못했습니다.");
            }
        }
    }

    // [수정 포인트 1-B] 반환 타입을 Vector2에서 Vector2? (Nullable)로 변경
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

        // 실패했을 때 Vector2.zero(0,0)가 아닌 null을 반환하여 완벽하게 실패를 알림
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