using System.Collections;
using System.Collections.Generic;
using BookCurlPro;
using DG.Tweening;
using UnityEngine;

public class NoteBookUI : MonoBehaviour
{
    [Header("노트 설정")]
    private RectTransform  _guideRectTransform;
    public float outPositionY = 1200f;
    public float animationDuration = 0.5f;
    private Vector2 _noteStartPosition;
    private bool _isAnimating;
    private bool _isOpen;

    [Header("연출 설정")] public float fadeDuration = 0.3f;

    public BookPro bookPro;
    public AutoFlip autoFlip;

    [Header("프리팹 연결")] 
    public GameObject leftPagePrefab; // 모서리 버튼이 좌측 하단에 있는 프리팹
    public GameObject rightPagePrefab;

    public void SetupNotebookData()
    {
        var unlockedData = ConstellationDataRepository.Instance.GetUnlockedData();

        _guideRectTransform = transform.GetComponent<RectTransform>();
        _noteStartPosition = _guideRectTransform.anchoredPosition;
        
        _guideRectTransform.anchoredPosition = new Vector2(_noteStartPosition.x, outPositionY);
        _isOpen = false;

        bookPro.Init();

        // 💡 1. 필요한 총 Paper 묶음 개수 계산
        // 데이터가 1개면 Page 1, Page 2가 필요하므로 papers[0]과 papers[1] 총 2묶음이 필요함.
        int neededPapers = unlockedData.Count + 2;

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
            var displayData = unlockedData[i];

            // --- 왼쪽 페이지 (사진) ---
            // 0번째 데이터 -> papers[0].Back (Page 1)
            // 1번째 데이터 -> papers[1].Back (Page 3)
            var leftUI = bookPro.papers[i+1].Back.GetComponent<NoteBookPageUI>();
            if (leftUI != null)
            {
                leftUI.SetPageData(displayData);
            }

            // --- 오른쪽 페이지 (글) ---
            // 0번째 데이터 -> papers[1].Front (Page 2)
            // 1번째 데이터 -> papers[2].Front (Page 4)
            var rightUI = bookPro.papers[i + 2].Front.GetComponent<NoteBookPageUI>();
            if (rightUI != null)
            {
                rightUI.SetPageData(displayData);
                // 마지막 데이터면 다음으로 넘어가는 버튼 숨기기
                // rightUI.SetDogEarButtonActive(i < unlockedData.Count - 1);
            }
        }

        // 💡 4. 책 안 닫히게 범위 설정
        bookPro.StartFlippingPaper = 0;
        bookPro.EndFlippingPaper = neededPapers - 1;

        // 수첩 켰을 때 바로 Page 1과 Page 2가 마주보게 펼치기
        bookPro.CurrentPaper = 2;
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
        if (_isAnimating) return;
    
        PlayVerticalSlide(!_isOpen);
    }
    private void PlayVerticalSlide(bool show)
    {
        gameObject.SetActive(true);
        _isAnimating = true;
        // 나올 때는 원래 위치로, 들어갈 때는 위쪽 바깥으로
        float targetY = show ? _noteStartPosition.y : outPositionY;
    
        // 나올 때는 툭 떨어지는 느낌(OutBounce 또는 OutExpo), 
        // 들어갈 때는 슉 올라가는 느낌(InCubic)
        Ease easeType = show ? Ease.OutExpo : Ease.InCubic;

        _guideRectTransform.DOKill();
        _guideRectTransform.DOAnchorPosY(targetY, animationDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => {
                _isAnimating = false;
                _isOpen = show;
                if (!show) gameObject.SetActive(false); 
            });
    }
}