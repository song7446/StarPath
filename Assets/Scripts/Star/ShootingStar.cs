using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(TrailRenderer))]
public class ShootingStar : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float lifeTime = 2.5f;
    public Vector2 direction = new Vector2(-1f, -1f);

    private SpriteRenderer sprite;
    private TrailRenderer trail;
    private float timer;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        timer = 0f;

        // 랜덤 방향과 속도 살짝 섞기
        direction = new Vector2(Random.Range(-1.2f, -0.8f), -1f).normalized;
        speed = Random.Range(5f, 9f);
        lifeTime = Random.Range(2.0f, 3.0f);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 이동
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // 투명도 점점 줄이기 (별 본체)
        if (sprite != null)
        {
            float fadeStart = lifeTime * 0.6f;
            float fadeT = Mathf.Clamp01((timer - fadeStart) / (lifeTime - fadeStart));
            Color c = sprite.color;
            c.a = Mathf.Lerp(1f, 0f, fadeT);
            sprite.color = c;
        }

        // 수명 끝나면 파괴
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}