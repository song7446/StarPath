using SongLib.Core.Bootstrap;
using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    private async void Start()
    {
        await DialogueDataRepository.Instance.LoadDialogueJsonSO();
        await ConstellationDataRepository.Instance.LoadConstellationData();

        await GameManager.Instance.EnterNextChapter();

        
        Destroy(this);
    }
}
