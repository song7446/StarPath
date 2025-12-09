using UnityEngine;
using System.Collections;

public class ShootingStarSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject shootingStarPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-10f, 10f);
    [SerializeField] private Vector2 spawnRangeY = new Vector2(4f, 6f);
    [SerializeField] private float spawnIntervalMin = 1.5f;
    [SerializeField] private float spawnIntervalMax = 4f;

    private bool _isActive;

    public void StartSpawning()
    {
        if (_isActive) return;
        _isActive = true;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        _isActive = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (_isActive)
        {
            SpawnStar();
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }

    private void SpawnStar()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y),
            0f
        );

        var star = Instantiate(shootingStarPrefab, spawnPos, Quaternion.identity);
    }
}