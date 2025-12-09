using UnityEngine;

public class TwinkleStarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject twinkleStarPrefab;
    [SerializeField] private int starCount = 20;
    [SerializeField] private Vector2 spawnArea = new Vector2(9f, 5f);

    private void Start()
    {
        for (int i = 0; i < starCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y),
                0f
            );
            Instantiate(twinkleStarPrefab, pos, Quaternion.identity, transform);
        }
    }
}