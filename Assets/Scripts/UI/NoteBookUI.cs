using System.Collections;
using UnityEngine;

public class NoteBookUI : MonoBehaviour
{
    [Header("수첩 UI 연결")]
    public CanvasGroup canvasGroup;     

    [Header("연출 설정")]
    public float fadeDuration = 0.3f;   

    private bool _isOpen = false;
    
    // 💡 현재 실행 중인 애니메이션 코루틴을 담아둘 변수
    private Coroutine _currentCoroutine; 

    public void ToggleNotebook()
    {
        // [안전장치 1] 만약 상위 부모(Canvas 등)가 아예 꺼져있다면 실행 안 함
        if (!gameObject.activeInHierarchy && transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("부모 UI가 꺼져 있어서 수첩을 켤 수 없습니다.");
            return;
        }

        _isOpen = !_isOpen; // 상태를 먼저 무조건 반전시킴

        // 💡 [안전장치 2] 이미 돌고 있는 페이드 애니메이션이 있다면 강제 정지! (광클 방지)
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if (_isOpen)
        {
            gameObject.SetActive(true); // 코루틴을 켜기 전에 무조건 활성화
            _currentCoroutine = StartCoroutine(FadeInAnimation());
        }
        else
        {
            // 꺼야 하는데 오브젝트가 이미 꺼져있다면 굳이 코루틴을 돌릴 필요 없음
            if (gameObject.activeInHierarchy)
            {
                _currentCoroutine = StartCoroutine(FadeOutAnimation());
            }
        }
    }

    private IEnumerator FadeInAnimation()
    {
        float time = 0f;
        
        // 💡 투명도를 무조건 0으로 안 하고, '현재 멈춘 투명도'에서 이어서 시작 (부드러운 전환)
        float startAlpha = canvasGroup.alpha; 

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float easeOut = 1f - Mathf.Pow(1f - t, 3f); 

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, easeOut);
            
            yield return null;
        }

        canvasGroup.alpha = 1f; 
        _currentCoroutine = null; // 연출 끝
    }

    private IEnumerator FadeOutAnimation()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float easeIn = t * t * t; 

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, easeIn);
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f; 
        gameObject.SetActive(false); // 완전히 투명해진 뒤에 꺼줌
        _currentCoroutine = null; // 연출 끝
    }
}