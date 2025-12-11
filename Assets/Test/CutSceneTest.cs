using UnityEngine;

public class CutSceneTest : MonoBehaviour
{
    [Header("테스트할 컷씬 데이터")]
    public CutSceneCast testCutScene;

    [Header("자동 재생 여부")]
    public bool playOnStart = true;

    private void Start()
    {
        if (playOnStart && testCutScene != null)
        {
            Debug.Log($"▶ 컷씬 자동 재생 시작: {testCutScene.name}");
            CutsceneManager.Instance.PlayCutscene(testCutScene);
        }
    }

    // 수동 실행용 — 에디터에서 InvokeButton 등으로 실행 가능
    [ContextMenu("Play Cutscene")]
    public void Play()
    {
        if (testCutScene == null)
        {
            Debug.LogWarning("⚠ 테스트용 컷씬 데이터가 지정되지 않았습니다!");
            return;
        }

        Debug.Log($"▶ 컷씬 수동 재생: {testCutScene.name}");
        CutsceneManager.Instance.PlayCutscene(testCutScene);
    }
}