using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongLib;
using SongLib.Core.Singleton;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarSpawnManager : MonoBehaviourSingleton<StarSpawnManager>, IGameInitializer
{
    [Header("스폰 설정")] 
    public float minDistance;
    public int maxSpawnAttempts ;
    public int fakeStarCount ;
    public GameObject starPrefab;

    [Header("카메라 세팅")] 
    [SerializeField] private CinemachineCamera ccStarSkyCamera;    // 진짜 별(정답) 카메라 (빨간 박스)
    [SerializeField] private CinemachineCamera ccStarGroundCamera; // 가짜 별(방해) 카메라 (노란 박스)

    private GameObject _constellationPrefab;
    private GameObject _constellationObj;
    private List<GameObject> _spawnedStars = new List<GameObject>();
    private List<Vector2> occupiedPositions = new List<Vector2>();
    
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

            // 💡 위에서 만든 빨간 영역 프로퍼티를 바로 가져와서 바닥 좌표를 구함!
            float redBottomY = ConstellationArea.yMin; 
            
            float yellowTopY = ccStarGroundCamera.transform.position.y + ortho;
            float yellowLeftX = ccStarGroundCamera.transform.position.x - (width / 2f);

            // 아래가 싹둑 잘린 완벽한 노란 영역 반환
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

    public void SpawnStar()
    {
        occupiedPositions.Clear();

        // 1. 진짜 별 스폰 (빨간 영역)
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

        // 2. 가짜 별 스폰 (노란 영역)
        Rect groundRect = FakeStarArea; 
        
        for (int i = 0; i < fakeStarCount; i++)
        {
            // 매개변수로 영역을 넘겨주어 책임을 명확히 분리
            Vector2? validPos = GetValidRandomPos(groundRect); 

            if (validPos.HasValue)
            {
                GameObject starObj = Instantiate(starPrefab, validPos.Value, Quaternion.identity, transform);
                _spawnedStars.Add(starObj);
                
                PuzzleStar star = starObj.GetComponent<PuzzleStar>();
                if (star != null) star.SetupStar(occupiedPositions.Count + 1, true);
                
                occupiedPositions.Add(validPos.Value);
            }
            else
            {
                Debug.LogWarning($"공간이 부족하여 {i + 1}번째 방해 별을 배치하지 못했습니다.");
            }
        }
    }

    private Vector2? GetValidRandomPos(Rect spawnRect)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnRect.xMin, spawnRect.xMax),
                Random.Range(spawnRect.yMin, spawnRect.yMax)
            );

            if (IsPositionSafe(randomPos)) return randomPos;
        }
        return null;
    }

    private bool IsPositionSafe(Vector2 pos)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(pos, occupiedPos) < minDistance) return false;
        }
        return true;
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
        // Calculate 부를 필요 없이 프로퍼티만 읽으면 실시간 적용 끝!
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