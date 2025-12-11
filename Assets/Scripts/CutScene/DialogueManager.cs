using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public DialogueDatabase dataBase;
    public TMP_Text dialogueText;
    private bool waiting = false;

    public void ShowDialogue(string id)
    {
        var entry = dataBase.GetLine(id);
        if (entry == null) return;
        
        dialogueText.text = LocalizationManager.Instance.GetLocalizedText(entry);
        dialogueText.gameObject.SetActive(true);
        StartCoroutine(WaitForClick());
    }

    private IEnumerator WaitForClick()
    {
        waiting = true;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        waiting = false;
        dialogueText.text = "";
    }

    public bool IsWaiting() => waiting;
}