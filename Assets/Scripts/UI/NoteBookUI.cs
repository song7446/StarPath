using System.Collections;
using System.Collections.Generic;
using BookCurlPro;
using UnityEngine;

public class NoteBookUI : MonoBehaviour
{
    [Header("수첩 UI 연결")] public CanvasGroup canvasGroup;

    [Header("연출 설정")] public float fadeDuration = 0.3f;

    private bool _isOpen = false;

    // 💡 현재 실행 중인 애니메이션 코루틴을 담아둘 변수
    private Coroutine _currentCoroutine;

    public BookPro bookPro;
    public AutoFlip autoFlip;

    [Header("프리팹 연결")] public GameObject leftPagePrefab; // 모서리 버튼이 좌측 하단에 있는 프리팹
    public GameObject rightPagePrefab;

    public void SetupNotebookData()
    {
        var unlockedData = ConstellationDataRepository.Instance.GetUnlockedData();

        bookPro.Init();

        // 💡 1. 필요한 총 Paper 묶음 개수 계산
        // 데이터가 1개면 Page 1, Page 2가 필요하므로 papers[0]과 papers[1] 총 2묶음이 필요함.
        int neededPapers = unlockedData.Count + 10;

        // 종이가 모자라면 동적으로 생성 (AddNewPaper 활용)
        while (bookPro.papers.Length < neededPapers)
        {
            AddNewPaper();
        }

        // 💡 2. 모든 페이지 UI 싹 지우기 (더미 방지)
        for (int i = 0; i < bookPro.papers.Length; i++)
        {
            var frontUI = bookPro.papers[i].Front.GetComponent<NoteBookPageUI>();
            if (frontUI != null)
            {
                frontUI.ClearPage();

                frontUI.SetActionOnClickDogEarButton(() => autoFlip.FlipRightPage());
                frontUI.SetDogEarButton();
            }

            var backUI = bookPro.papers[i].Back.GetComponent<NoteBookPageUI>();
            if (backUI != null)
            {
                backUI.ClearPage();

                backUI.SetActionOnClickDogEarButton(() => autoFlip.FlipLeftPage());
                backUI.SetDogEarButton();
            }
        }

        // 💡 3. 하이라키 상의 Page 번호 흐름대로 착착 꽂아 넣기
        for (int i = 0; i < unlockedData.Count; i++)
        {
            var data = unlockedData[i];

            // --- 왼쪽 페이지 (사진) ---
            // 0번째 데이터 -> papers[0].Back (Page 1)
            // 1번째 데이터 -> papers[1].Back (Page 3)
            var leftUI = bookPro.papers[i].Back.GetComponent<NoteBookPageUI>();
            if (leftUI != null)
            {
                leftUI.SetPageData(data);
            }

            // --- 오른쪽 페이지 (글) ---
            // 0번째 데이터 -> papers[1].Front (Page 2)
            // 1번째 데이터 -> papers[2].Front (Page 4)
            var rightUI = bookPro.papers[i + 1].Front.GetComponent<NoteBookPageUI>();
            if (rightUI != null)
            {
                rightUI.SetPageData(data);
                // 마지막 데이터면 다음으로 넘어가는 버튼 숨기기
                // rightUI.SetDogEarButtonActive(i < unlockedData.Count - 1);
            }
        }

        // 💡 4. 책 안 닫히게 범위 설정
        bookPro.StartFlippingPaper = 0;
        bookPro.EndFlippingPaper = neededPapers - 1;

        // 수첩 켰을 때 바로 Page 1과 Page 2가 마주보게 펼치기
        bookPro.CurrentPaper = 0;
    }

    public void AddNewPaper()
    {
        // 💡 [핵심] BookPro에서 Front는 오른쪽 면, Back은 왼쪽 면입니다!
        GameObject newFront = Instantiate(rightPagePrefab, bookPro.transform); // 오른쪽
        GameObject newBack = Instantiate(leftPagePrefab, bookPro.transform); // 왼쪽

        newFront.name = "Page" + (bookPro.papers.Length * 2);
        newBack.name = "Page" + (bookPro.papers.Length * 2 + 1);

        Paper newPaper = new Paper();
        newPaper.Front = newFront;
        newPaper.Back = newBack;

        List<Paper> paperList = new List<Paper>(bookPro.papers);
        paperList.Add(newPaper);
        bookPro.papers = paperList.ToArray();

        bookPro.UpdatePages();
    }

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