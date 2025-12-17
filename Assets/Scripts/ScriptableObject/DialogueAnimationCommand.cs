using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Animation")]
public class DialogueAnimationAsset : ScriptableObject
{
    public string dialogueId;
    
    [System.Serializable]
    public class DialogueAnimationCommand
    {
        public string actorId;
        public string trigger;
        public bool waitEnd;
    }

    public DialogueAnimationCommand[] commands;
}