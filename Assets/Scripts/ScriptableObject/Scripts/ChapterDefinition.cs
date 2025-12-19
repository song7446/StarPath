using UnityEngine;

[CreateAssetMenu(menuName = "Game/Chapter")]
public class ChapterDefinition : ScriptableObject
{
    [Header("Chapter Info")] public string chapterId;

    [Header("Dialogue Animations")] public DialogueAnimationAsset[] dialogueAnimations;
    
    [Header("Characters")] public CutSceneCast cutSceneCasts;
}