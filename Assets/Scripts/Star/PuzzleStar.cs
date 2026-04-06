using System;
using UnityEngine;

public class PuzzleStar : MonoBehaviour,IInteractable
{
    public int starID; // 노트의 정답과 비교할 고유 번호
    public bool isCorrect;
    
    public void SetupStar(int id, bool correct)
    {
        starID = id;
        isCorrect = correct;
        
        // 정답 별인지 방해 별인지에 따라 색상이나 크기를 살짝 다르게 할 수도 있음
        if(!isCorrect) { /* 방해 별 설정 */ }
    }
    
    public void OnInteract()
    {
        ConstellationManager.Instance.StartDrawing(this);
        Debug.Log("Click");
    }
}