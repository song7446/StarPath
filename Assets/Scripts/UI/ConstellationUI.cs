using System;
using DG.Tweening;
using SongLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstellationUI : MonoBehaviour
{
    [Header("UI 연결")] 
    public GameObject GuidePanel;
    public Image GuideImage; // 폴라로이드 이미지
    public TextMeshProUGUI GuideText;

    [Header("설정")]
    public float animationDuration = 0.5f;
    public float outPositionX = -530;

    private Vector2 startPosition;
    private bool isAnimating = false;
    private bool isOnGuide = false; // 현재 가이드가 화면에 나와있는지 여부
    private bool isImageVisible = true; // 폴라로이드 이미지가 켜져있는지 여부

    private RectTransform guideRectTransform;

    public void Init()
    {
        guideRectTransform = GuidePanel.GetComponent<RectTransform>();
        startPosition = guideRectTransform.anchoredPosition;

        // [추가] 시작할 때 패널을 화면 밖으로 미리 빼둡니다.
        guideRectTransform.anchoredPosition = new Vector2(outPositionX, startPosition.y);
        isOnGuide = false;
    }

    public void UpdateGuide(ConstellationData constellationData)
    {
        if (isAnimating || GuideImage == null) return;

        if (constellationData == null)
        {
            HideGuide();
            return;
        }

        if (!GuidePanel.activeSelf)
        {
            // 처음 나타날 때는 바로 등장
            GuidePanel.SetActive(true);
            ChangeInfo(constellationData);
            PlaySlideAnimation(true); // 안으로 들어오기
        }
        else
        {
            // 이미 있으면 나갔다(데이터 교체) 들어오기
            PlaySwapAnimation(constellationData);
        }
    }

    public void ToggleGuideImage()
    {
        if (isAnimating) return;

        // 현재 상태(isOnGuide)의 반대로 움직임
        PlaySlideAnimation(!isOnGuide);
    }

// --- 공통 연출 함수들 ---

// 1. 단순히 넣었다 뺐다 하는 애니메이션
    private void PlaySlideAnimation(bool show)
    {
        gameObject.SetActive(true);
        isAnimating = true;
        float targetX = show ? startPosition.x : outPositionX;
        Ease easeType = show ? Ease.OutExpo : Ease.InCubic;

        guideRectTransform.DOAnchorPosX(targetX, animationDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                isAnimating = false;
                isOnGuide = show;
                if (!show) gameObject.SetActive(false);
            });
    }

// 2. 나갔다가 정보를 바꾸고 다시 들어오는 애니메이션 (Update 전용)
    private void PlaySwapAnimation(ConstellationData data)
    {
        isAnimating = true;
        Sequence swapSequence = DOTween.Sequence();

        swapSequence.Append(guideRectTransform.DOAnchorPosX(outPositionX, animationDuration).SetEase(Ease.InQuad));
        swapSequence.AppendCallback(() => ChangeInfo(data));
        swapSequence.Append(guideRectTransform.DOAnchorPosX(startPosition.x, animationDuration).SetEase(Ease.OutQuad));

        swapSequence.OnComplete(() =>
        {
            isAnimating = false;
            isOnGuide = true;
        });
    }

    private void HideGuide()
    {
        isAnimating = true;
        GuidePanel.GetComponent<RectTransform>()
            .DOAnchorPosX(outPositionX, animationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                GuidePanel.SetActive(false);
                isAnimating = false;
                isOnGuide = false;
            });
    }

    private void ChangeInfo(ConstellationData constellationData)
    {
        GuideImage.sprite = constellationData.constellationGuideImage;

        ConstellationRow text =
            ConstellationDataRepository.Instance.GetConstellationRow(constellationData.constellationId);

        GuideText.text = text.Name_Kr;

        // 새로운 데이터가 들어올 때 이미지가 꺼져있었다면 다시 키게 할지, 
        // 아니면 꺼진 상태를 유지할지는 원석님 기획에 따라 선택하시면 됩니다.
        if (!isImageVisible)
        {
            isImageVisible = true;
            GuideImage.rectTransform.localScale = Vector3.one;
        }
    }
}