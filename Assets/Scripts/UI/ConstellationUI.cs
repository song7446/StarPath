using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstellationUI : MonoBehaviour
{
    [Header("UI 연결")] 
    public GameObject GuidePanel;
    public Image GuideImage;
    public TextMeshProUGUI GuideText;
    
    // 💡 챕터가 시작될 때 호출할 함수
    public void UpdateGuide(ConstellationData constellationData)
    {
        if (GuideImage == null) return;
        
        if (constellationData == null)
        {
            GuidePanel.SetActive(false);
            return;
        }
        
        GuideImage.sprite = constellationData.constellationGuideImage;

        ConstellationRow text =
            ConstellationDataRepository.Instance.GetConstellationRow(constellationData.constellationId);
        
        GuideText.text = text.Name_Kr;
        
        GuidePanel.SetActive(true);
    }
}