using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FakeStarMode
{
    None, // 튜토리얼: 방해 별 생성 안 함
    Cluster, // 초중반: 정답 별자리 주변에만 뭉쳐서 생성
    Global // 후반부: 맵 전체에 포아송으로 흩뿌리기
}

public class StarSpawnManager : MonoBehaviourSingleton<StarSpawnManager>, IGameInitializer
{
    [Header("난이도 설정")] [Tooltip("현재 스테이지에 맞는 스폰 모드를 선택하세요.")]
    public FakeStarMode currentSpawnMode = FakeStarMode.Cluster;

    [Header("스폰 설정")] 
    public int maxSpawnAttempts = 30; // 💡 (참고: 이제 포아송과 뭉치기 로직이 생겨서 잘 안 쓰이지만, 안전장치로 남겨둠)
    public GameObject starPrefab;

    [Header("카메라 세팅")] [SerializeField] private CinemachineCamera ccStarSkyCamera; // 진짜 별(정답) 카메라 (빨간 박스)
    [SerializeField] private CinemachineCamera ccStarGroundCamera; // 가짜 별(방해) 카메라 (노란 박스)

    private GameObject _constellationPrefab;
    private GameObject _constellationObj;
    private List<GameObject> _spawnedStars = new List<GameObject>();
    private List<Vector2> occupiedPositions = new List<Vector2>();

    [Header("방해 별 동적 세팅")] [Tooltip("정답 별자리 간격에 곱할 가중치 (1.0 = 동일, 0.9 = 약간 더 촘촘함)")]
    public float spacingMultiplier;

    [Tooltip("간격이 너무 좁아져서 렉이 걸리는 것을 방지하는 최소값")]
    public float minSpacingLimit;

    [Tooltip("간격이 너무 넓어져서 맵이 휑해지는 것을 방지하는 최대값")]
    public float maxSpacingLimit;

    [Header("방해 별 세팅")] [Tooltip("맵에 뿌려질 최대 가짜 별 개수 (Cluster/Global 모드 공통)")]
    public int fakeStarCount;

    [Tooltip("정답 별자리 주변에 찰싹 붙여서 모양을 숨길 가짜 별의 개수")]
    public int localCamouflageCount;

    public Rect ConstellationArea
    {
        get
        {
            if (ccStarSkyCamera == null) return Rect.zero;

            float ortho = ccStarSkyCamera.Lens.OrthographicSize;
            float height = ortho * 2f;
            float width = height * (16f / 9f);

            Vector2 pos = ccStarSkyCamera.transform.position;
            return new Rect(pos.x - (width / 2f), pos.y - (height / 2f), width, height);
        }
    }

    public Rect FakeStarArea
    {
        get
        {
            if (ccStarGroundCamera == null || ccStarSkyCamera == null) return Rect.zero;

            float ortho = ccStarGroundCamera.Lens.OrthographicSize;
            float width = ortho * 2f * (16f / 9f);

            float redBottomY = ConstellationArea.yMin;
            float yellowTopY = ccStarGroundCamera.transform.position.y + ortho;
            float yellowLeftX = ccStarGroundCamera.transform.position.x - (width / 2f);

            return new Rect(yellowLeftX, redBottomY, width, yellowTopY - redBottomY);
        }
    }

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

    // 외부(스테이지 매니저 등)에서 난이도(모드)를 바꿀 때 부르는 함수
    public void SetSpawnMode(FakeStarMode mode)
    {
        currentSpawnMode = mode;
    }

    public void SpawnStar()
    {
        occupiedPositions.Clear();

        // ---------------------------------------------------
        // 1. 진짜 별 스폰 (빨간 영역) - 항상 실행됨
        // ---------------------------------------------------
        Rect skyRect = ConstellationArea;

        Vector2 constellationCenter = new Vector2(
            skyRect.center.x + Random.Range(-skyRect.width / 6f, skyRect.width / 6f),
            skyRect.center.y + Random.Range(-skyRect.height / 6f, skyRect.height / 6f)
        );

        _constellationObj = Instantiate(_constellationPrefab, constellationCenter, Quaternion.identity, transform);

        foreach (Transform child in _constellationObj.transform)
        {
            PuzzleStar star = child.GetComponent<PuzzleStar>();
            if (star != null)
            {
                star.SetupStar(occupiedPositions.Count + 1, true);
                occupiedPositions.Add(child.position);
            }
        }

        // 💡 튜토리얼 모드면 여기서 바로 종료! (방해 별 안 만듦)
        if (currentSpawnMode == FakeStarMode.None)
        {
            Debug.Log("[튜토리얼 모드] 정답 별자리만 스폰되었습니다.");
            return;
        }

        int realStarCount = occupiedPositions.Count;
        float rawAverageSpacing = CalculateAverageStarSpacing(occupiedPositions);
        float dynamicSpacing = Mathf.Clamp(rawAverageSpacing * spacingMultiplier, minSpacingLimit, maxSpacingLimit);

        // ---------------------------------------------------
        // 💡 3. 선택된 모드에 맞춰 가짜 별 스폰 분기 처리
        // ---------------------------------------------------
        switch (currentSpawnMode)
        {
            case FakeStarMode.Cluster:
                SpawnFakeStarsCluster(dynamicSpacing);
                break;
            case FakeStarMode.Global:
                SpawnFakeStarsGlobal(dynamicSpacing, realStarCount);
                break;
        }
    }

    // =========================================================
    // 모드 1. 퍼져나가기 (주변 뭉치기) 스폰
    // =========================================================
    private void SpawnFakeStarsCluster(float dynamicSpacing)
    {
        Rect groundRect = FakeStarArea;
        int fakeSpawned = 0;

        for (int i = 0; i < fakeStarCount; i++)
        {
            bool spawnedInThisTurn = false;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector2 baseStarPos = occupiedPositions[Random.Range(0, occupiedPositions.Count)];
                float angle = Random.value * Mathf.PI * 2f;
                float dist = Random.Range(dynamicSpacing * 1.0f, dynamicSpacing * 2.0f);
                Vector2 candidatePos = baseStarPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (!groundRect.Contains(candidatePos)) continue;

                if (IsPositionSafe(candidatePos, dynamicSpacing * 0.8f))
                {
                    InstantiateFakeStar(candidatePos);
                    fakeSpawned++;
                    spawnedInThisTurn = true;
                    break;
                }
            }

            if (!spawnedInThisTurn) break;
        }

        Debug.Log($"[Cluster 모드] 별자리 주변 뭉치기 완료: {fakeSpawned}개");
    }

    // =========================================================
    // 모드 2. 포아송 디스크 (맵 전체 흩뿌리기) + 집중 위장 스폰
    // =========================================================
    private void SpawnFakeStarsGlobal(float dynamicSpacing, int realStarCount)
    {
        int localSpawned = 0;

        // 2.5 집중 위장 (Camouflage)
        for (int i = 0; i < localCamouflageCount; i++)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector2 targetRealStar = occupiedPositions[Random.Range(0, realStarCount)];
                float angle = Random.value * Mathf.PI * 2f;
                float randomDist = Random.Range(dynamicSpacing * 0.8f, dynamicSpacing * 1.5f);
                Vector2 localPos = targetRealStar + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomDist;

                if (IsPositionSafe(localPos, dynamicSpacing * 1f))
                {
                    InstantiateFakeStar(localPos);
                    localSpawned++;
                    break;
                }
            }
        }


        // 3. 포아송 배경 노이즈
        Rect groundRect = FakeStarArea;
        List<Vector2> candidatePositions = PoissonDiscSampler.GeneratePoints(groundRect, dynamicSpacing);

        List<Vector2> safePositions = new List<Vector2>();
        foreach (Vector2 pos in candidatePositions)
        {
            if (IsPositionSafe(pos, dynamicSpacing * 0.8f))
            {
                safePositions.Add(pos);
            }
        }

        // 셔플 (랜덤하게 섞기)
        for (int i = 0; i < safePositions.Count; i++)
        {
            Vector2 temp = safePositions[i];
            int randomIndex = Random.Range(i, safePositions.Count);
            safePositions[i] = safePositions[randomIndex];
            safePositions[randomIndex] = temp;
        }

        int spawnLimit = Mathf.Min(fakeStarCount, safePositions.Count);
        int globalSpawnedCount = 0;

        for (int i = 0; i < spawnLimit; i++)
        {
            InstantiateFakeStar(safePositions[i]);
            globalSpawnedCount++;
        }

        Debug.Log($"[Global 모드] 집중 위장 별: {localSpawned}개 / 배경 노이즈 별: {globalSpawnedCount}개");
    }

    // ---------------------------------------------------------
    // 유틸리티 함수들 (중복 방지용)
    // ---------------------------------------------------------
    private void InstantiateFakeStar(Vector2 pos)
    {
        GameObject starObj = Instantiate(starPrefab, pos, Quaternion.identity, transform);
        _spawnedStars.Add(starObj);

        PuzzleStar star = starObj.GetComponent<PuzzleStar>();
        if (star != null) star.SetupStar(occupiedPositions.Count + 1, true);

        occupiedPositions.Add(pos);
    }

    // 💡 참고: 기존에 있던 파라미터 없는 IsPositionSafe와 GetValidRandomPos는 
    // 동적 거리를 사용하는 현재 구조에서 더 이상 쓰이지 않아 깔끔하게 삭제했습니다.
    private bool IsPositionSafe(Vector2 pos, float safeDistance)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(pos, occupiedPos) < safeDistance) return false;
        }

        return true;
    }

    private float CalculateAverageStarSpacing(List<Vector2> starPositions)
    {
        if (starPositions.Count < 2) return 2.5f;

        float totalMinDistance = 0f;

        for (int i = 0; i < starPositions.Count; i++)
        {
            float minDistance = float.MaxValue;
            for (int j = 0; j < starPositions.Count; j++)
            {
                if (i == j) continue;
                float dist = Vector2.Distance(starPositions[i], starPositions[j]);
                if (dist < minDistance) minDistance = dist;
            }

            totalMinDistance += minDistance;
        }

        return totalMinDistance / starPositions.Count;
    }

    public async Task FadeOutAllStars(float duration = 1.0f)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        foreach (var star in _spawnedStars)
        {
            if (star != null)
            {
                var sr = star.GetComponent<SpriteRenderer>();
                if (sr != null) renderers.Add(sr);
            }
        }

        if (renderers.Count == 0) return;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / duration);

            foreach (var sr in renderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = alpha;
                    sr.color = c;
                }
            }

            await Task.Yield();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ccStarSkyCamera != null)
        {
            Rect skyRect = ConstellationArea;
            Vector3 center = new Vector3(skyRect.center.x, skyRect.center.y, transform.position.z);
            Vector3 size = new Vector3(skyRect.width, skyRect.height, 0.1f);

            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
        }

        if (ccStarGroundCamera != null && ccStarSkyCamera != null)
        {
            Rect groundRect = FakeStarArea;
            Vector3 center = new Vector3(groundRect.center.x, groundRect.center.y, transform.position.z);
            Vector3 size = new Vector3(groundRect.width, groundRect.height, 0.1f);

            Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f);
            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, size);
        }
    }
}