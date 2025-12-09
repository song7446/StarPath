using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    public float speed = 6f;
    public float lifeTime = 2.5f;
    public Vector2 direction = new Vector2(-1f, -1f);

    private SpriteRenderer sprite;
    private float timer;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Fade out (수명 후반부에 점점 투명해짐)
        if (sprite != null)
        {
            float fadeStart = lifeTime * 0.6f;
            float fadeT = Mathf.Clamp01((timer - fadeStart) / (lifeTime - fadeStart));
            Color c = sprite.color;
            c.a = Mathf.Lerp(1f, 0f, fadeT);
            sprite.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}