using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class NoteBookPageUI : MonoBehaviour
{
    [Header("페이지 타입")]
    public bool isLeftPage; // 왼쪽 프리팹은 true, 오른쪽 프리팹은 false로 체크

    [Header("왼쪽 페이지 전용 (홀수)")]
    public Image constellationImage;
    public Button polaroidButton; // 폴라로이드 가이드 변경 버튼
    public TextMeshProUGUI nameText;

    [Header("오른쪽 페이지 전용 (짝수)")]
    public TextMeshProUGUI descText;

    [Header("페이지 넘김 버튼 (모서리)")]
    public Button dogEarButton; // 45도 마스크로 만든 모서리 버튼 오브젝트
    public Action OnClickDogEarButton;
    
    private ConstellationDisplayData _constellationDisplayData;

    public void SetActionOnClickDogEarButton(Action action)
    {
        OnClickDogEarButton += action;
    }

    public void SetDogEarButton()
    {
        dogEarButton.onClick.AddListener(OnClickDogEarButton.Invoke);
    }

    public void SetPageData(ConstellationDisplayData constellationDisplayData)
    {
        _constellationDisplayData = constellationDisplayData;

        // if (isLeftPage)
        // {
        //     if (constellationImage != null)
        //     {
        //         constellationImage.sprite = constellationDisplayData.Data.constellationGuideImage;
        //         constellationImage.gameObject.SetActive(true);
        //     }
        //     
        //     if (nameText != null) nameText.text = constellationDisplayData.Row.Name_Kr;
        //
        //     if (polaroidButton != null)
        //     {
        //         polaroidButton.gameObject.SetActive(true);
        //         polaroidButton.onClick.RemoveAllListeners();
        //         polaroidButton.onClick.AddListener(OnClickPolaroidButton);
        //     }
        // }
        // else // 오른쪽 페이지
        // {
        //     if (descText != null) descText.text = constellationDisplayData.Row.Explain_Kr;
        // }
    }

    public void ClearPage()
    {
        _constellationDisplayData = null;
        if (constellationImage != null) constellationImage.gameObject.SetActive(false);
        if (polaroidButton != null) polaroidButton.gameObject.SetActive(false);
        if (nameText != null) nameText.text = "";
        if (descText != null) descText.text = "";
    }

    // 📸 별자리 사진 아래의 폴라로이드 버튼을 눌렀을 때
    private void OnClickPolaroidButton()
    {
        if (_constellationDisplayData == null) return;
        
        GameSceneUIManager.Instance.UpdateGuide(_constellationDisplayData.Data);
        
        Debug.Log($"{_constellationDisplayData.Row.Name_Kr} 폴라로이드 가이드로 변경!");
    }

    // 첫 장, 끝 장에서 모서리 버튼을 숨기기 위한 함수
    public void SetDogEarButtonActive(bool isActive)
    {
        if (dogEarButton != null)
        {
            dogEarButton.gameObject.SetActive(isActive);
        }
    }

    public ConstellationDisplayData GetPageConstellationDisplayData()
    {
        return _constellationDisplayData;
    }
}