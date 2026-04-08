using System.Threading.Tasks;
using SongLib;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    [Header("이번 스테이지 어드레서블 주소")]
    // 아까 Groups 창에서 지어준 이름표(Address)를 똑같이 적어줍니다.
    public string currentStageAddress; 

    private async void Start()
    {
        // 게임 시작 시 정답 데이터를 불러옵니다.
        await LoadAndSetupPuzzle();
    }

    private async Task LoadAndSetupPuzzle()
    {
        Debug.Log("정답 데이터 비동기 로딩 시작...");

        // 1. SongLib의 어드레서블 매니저를 통해 SO 데이터를 비동기로 가져옴
        ConstellationData answerData = await AddressableManager.LoadAssetAsync<ConstellationData>(currentStageAddress);

        // 2. 로딩 성공 시 매니저에 주입
        if (answerData != null)
        {
            Debug.Log($"로딩 성공! 이번 퍼즐: {answerData.constellationName}");
            
            // ConstellationManager가 "아, 이번 정답은 이거구나" 하고 세팅하게 됨
            ConstellationManager.Instance.SetCurrentPuzzle(answerData);

            // (선택) 여기에 별들을 화면에 뿌려주는 스폰 로직을 이어서 호출하면 완벽합니다.
            // StarSpawner.Instance.SpawnStars(answerData);
        }
    }

    private void OnDestroy()
    {
        AddressableManager.ReleaseAsset(currentStageAddress);
    }
}
