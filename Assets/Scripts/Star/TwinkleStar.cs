using UnityEngine;

public class TwinkleStar : MonoBehaviour
{
    [Header("Twinkle Settings")] public float minWait = 0.5f; // 반짝임 대기 최소
    public float maxWait = 3.0f; // 반짝임 대기 최대
    public float twinkleDuration = 0.6f; // 반짝임 지속 시간
    public float maxStretch = 5f; // 십자가로 퍼질 때 최대 스케일 배율
    public float minScale = 0.1f; // 기본 크기
    
    private float timer;
    private float waitTime;
    private bool isTwinkling;

    [SerializeField] private Transform starX;
    [SerializeField] private Transform starY;

    private void Awake()
    {
        ResetTwinkle();
    }

    private void ResetTwinkle()
    {
        timer = 0f;
        isTwinkling = false;
        waitTime = Random.Range(minWait, maxWait);
    }

    private void Update()
    {
        if (!isTwinkling)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                isTwinkling = true;
                timer = 0f;
            }
            return;
        }

        // 반짝이는 중
        timer += Time.deltaTime;
        float t = timer / twinkleDuration;

        // 반짝임 진행 비율 (0~1)
        float curve = Mathf.Sin(t * Mathf.PI); // 0→1→0 패턴
        
        float scale = Mathf.Lerp(minScale, maxStretch, curve);

        starX.localScale = new Vector3(scale, minScale, minScale);
        starY.localScale = new Vector3(minScale, scale, minScale);

        // 반짝임 끝나면 리셋
        if (timer >= twinkleDuration)
        {
            ResetTwinkle();
        }
    }
}