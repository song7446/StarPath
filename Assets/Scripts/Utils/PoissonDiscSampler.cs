using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampler
{
    /// <summary>
    /// 주어진 Rect 영역 안에 최소 거리(radius)를 보장하는 균일한 점들을 생성합니다.
    /// </summary>
    /// <param name="region">점들을 생성할 사각형 영역 (Rect)</param>
    /// <param name="radius">점들 사이의 최소 보장 거리</param>
    /// <param name="rejectionLimit">새로운 점을 찾기 위해 시도할 최대 횟수 (보통 30이 적당함)</param>
    /// <returns>생성된 점(Vector2)들의 리스트</returns>
    public static List<Vector2> GeneratePoints(Rect region, float radius, int rejectionLimit = 30)
    {
        float cellSize = radius / Mathf.Sqrt(2); // 그리드 셀 크기 계산
        
        int gridWidth = Mathf.CeilToInt(region.width / cellSize);
        int gridHeight = Mathf.CeilToInt(region.height / cellSize);
        
        // 점들이 들어갈 2D 그리드 (인덱스 저장용, 0은 빈 칸을 의미하도록 하기 위해 리스트 인덱스+1을 저장)
        int[,] grid = new int[gridWidth, gridHeight];
        
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        // 시작점 하나를 랜덤하게 중앙 근처에 생성
        spawnPoints.Add(region.center); 

        while (spawnPoints.Count > 0)
        {
            // 처리할 중심점 하나를 무작위로 선택
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < rejectionLimit; i++)
            {
                // 중심점에서 최소 거리(radius) ~ 최대 거리(2 * radius) 사이의 랜덤한 방향과 거리의 후보 점 생성
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                float randDistance = Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + dir * randDistance;

                // 후보 점이 영역(Rect) 안에 들어오는지 확인
                if (region.Contains(candidate))
                {
                    if (IsValid(candidate, region, cellSize, radius, points, grid))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        
                        // 그리드 좌표 계산 후 등록 (1-based index)
                        int cellX = Mathf.FloorToInt((candidate.x - region.xMin) / cellSize);
                        int cellY = Mathf.FloorToInt((candidate.y - region.yMin) / cellSize);
                        grid[cellX, cellY] = points.Count; 
                        
                        candidateAccepted = true;
                        break;
                    }
                }
            }

            // 시도 횟수 내에 적절한 점을 못 찾았다면 이 기준점은 폐기
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    private static bool IsValid(Vector2 candidate, Rect region, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        int cellX = Mathf.FloorToInt((candidate.x - region.xMin) / cellSize);
        int cellY = Mathf.FloorToInt((candidate.y - region.yMin) / cellSize);
        
        // 주변 5x5 셀을 검색하여 거리가 가까운 점이 있는지 확인
        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x <= searchEndX; x++)
        {
            for (int y = searchStartY; y <= searchEndY; y++)
            {
                int pointIndex = grid[x, y] - 1;
                if (pointIndex != -1)
                {
                    float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                    if (sqrDst < radius * radius)
                    {
                        return false; // 너무 가까운 점이 있으면 실패!
                    }
                }
            }
        }
        return true;
    }
}