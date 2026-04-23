using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using SongLib.Core.Singleton;

public class UITransition : MonoBehaviourSingleton<UITransition>
{
    [Header("아이리스 트랜지션 이미지")]
    [SerializeField] private RawImage transitionImage; 
    
    // 쉐이더 변수 캐싱 (이름이 쉐이더의 Reference와 완벽히 같아야 합니다)
    private readonly int RadiusProperty = Shader.PropertyToID("_Radius");
    private readonly int AspectRatioProperty = Shader.PropertyToID("_AspectRatio"); // 💡 추가된 화면 비율 변수

    /// <summary>
    /// 화면을 동그랗게 엽니다. (반지름 0 -> 1.5)
    /// </summary>
    public async Task OpenIris(float duration = 1.5f)
    {
        Debug.Log("화면 열기 시작");
        Material mat = transitionImage.material;
        float elapsed = 0f;
        
        // 시작 시점의 반지름을 확실하게 0으로 고정
        mat.SetFloat(RadiusProperty, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 0(완전 까맘)에서 1.5(모서리까지 완벽하게 열림)까지 증가
            float currentRadius = Mathf.Lerp(0f, 1.5f, elapsed / duration);
            
            // 💡 핵심: 매 프레임마다 현재 화면의 가로세로 비율을 쉐이더로 전달 (창모드 크기 조절 완벽 대응)
            mat.SetFloat(AspectRatioProperty, (float)Screen.width / Screen.height);
            mat.SetFloat(RadiusProperty, currentRadius);
            
            await Task.Yield();
        }
        
        // 루프 종료 후 최종 상태 쐐기 박기
        mat.SetFloat(AspectRatioProperty, (float)Screen.width / Screen.height);
        mat.SetFloat(RadiusProperty, 1.5f); 
    }

    /// <summary>
    /// 화면을 동그랗게 닫습니다. (반지름 1.5 -> 0)
    /// </summary>
    public async Task CloseIris(float duration = 1.5f)
    {
        Debug.Log("화면 닫기 시작");
        Material mat = transitionImage.material;
        float elapsed = 0f;
        
        mat.SetFloat(RadiusProperty, 1.5f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentRadius = Mathf.Lerp(1.5f, 0f, elapsed / duration);
            
            mat.SetFloat(AspectRatioProperty, (float)Screen.width / Screen.height);
            mat.SetFloat(RadiusProperty, currentRadius);
            
            await Task.Yield();
        }
        
        mat.SetFloat(RadiusProperty, 0f); // 완전 암전 상태 보장
    }
}