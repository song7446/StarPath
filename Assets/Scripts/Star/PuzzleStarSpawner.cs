using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleStarSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public Vector2 spawnAreaSize = new Vector2(15f, 10f); // 별이 스폰될 전체 영역 크기
    public float minDistance = 1.5f; // 별들 사이의 최소 거리 (이 값이 클수록 널널하게 배치됨)
    public int maxSpawnAttempts = 30; // 무한루프 방지용 최대 시도 횟수
    
    public ConstellationData currentData; // 현재 풀 퍼즐 데이터
    public GameObject constellationPrefab;
    public GameObject starPrefab;
    public int fakeStarCount = 10; // 방해 별 개수
    
    private List<Vector2> occupiedPositions = new List<Vector2>();

    public void SpawnStar()
    {
        occupiedPositions.Clear();
        // 1. 정답 별 스폰 (SO에 정의된 개수만큼)
        Vector2 constellationCenter = new Vector2(
            Random.Range(-spawnAreaSize.x / 4f, spawnAreaSize.x / 4f), 
            Random.Range(-spawnAreaSize.y / 4f, spawnAreaSize.y / 4f)
        );
        
        GameObject constellationObj = Instantiate(constellationPrefab, constellationCenter, Quaternion.identity);
        
        foreach (Transform child in constellationObj.transform)
        {
            PuzzleStar star = child.GetComponent<PuzzleStar>();
            if (star != null)
            {
                occupiedPositions.Add(child.position);
            }
        }
        
        // 2. 방해 별 스폰
        for (int i = 0; i < fakeStarCount; i++)
        {
            Vector2 validPos = GetValidRandomPos();

            // 유효한 위치를 찾았다면 스폰
            if (validPos != Vector2.zero || occupiedPositions.Count == 0) // zero는 실패 처리용 (임시)
            {
                Instantiate(starPrefab, validPos, Quaternion.identity);
                occupiedPositions.Add(validPos); // 방금 스폰한 가짜 별의 위치도 '차지된 자리'로 등록
            }
            else
            {
                Debug.LogWarning("더 이상 별을 배치할 빈 공간이 없습니다! minDistance나 스폰 영역을 조절하세요.");
            }
        }
    }
    
    private Vector2 GetValidRandomPos()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // 영역 내에서 랜덤 좌표 하나 픽
            Vector2 randomPos = new Vector2(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
            );

            // 해당 좌표가 기존 별들과 너무 가깝지 않은지 검사
            if (IsPositionSafe(randomPos))
            {
                return randomPos; // 안전하다면 바로 리턴!
            }
        }

        // maxSpawnAttempts 만큼 시도했는데도 못 찾았다면 Vector2.zero 리턴
        return Vector2.zero; 
    }

    // 모든 기존 별들과의 거리를 재서 안전한지 체크하는 함수
    private bool IsPositionSafe(Vector2 pos)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(pos, occupiedPos) < minDistance)
            {
                return false; // 하나라도 너무 가까운 별이 있으면 탈락
            }
        }
        return true; // 모두 통과했다면 안전한 자리!
    }

    // 유니티 에디터에서 스폰 영역을 시각적으로 보기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}