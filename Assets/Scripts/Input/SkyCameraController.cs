using UnityEngine;
using Unity.Cinemachine;

public class SkyCameraController : MonoBehaviour
{
    [Header("이동 속도 설정")] public float panSpeed = 15f;

    [Tooltip("마우스가 화면 끝에서 몇 픽셀 안에 들어와야 움직일지 결정 (기본 20)")]
    public float edgePanBorderThickness = 20f;

    public bool enableEdgePanning = true;

    [Header("카메라 세팅")] public CinemachineCamera skyCamera;

    private void Update()
    {
        if (!GameSceneUIManager.Instance.IsSkyView) return;
        
        // 1. 이제 UnityEngine.Input 대신 InputHandler를 통해 방향을 구합니다!
        Vector3 moveDir = GetInputDirection();

        // 2. 방향이 있으면 그쪽으로 이동
        if (moveDir.sqrMagnitude > 0)
        {
            transform.Translate(moveDir.normalized * panSpeed * Time.deltaTime);
            ClampPosition();
        }
    }

    private Vector3 GetInputDirection()
    {
        Vector3 dir = Vector3.zero;

        // 💡 1. InputHandler에서 키보드/게임패드 입력 받아오기
        // (원석님의 InputHandler에 맞게 프로퍼티/메서드 이름을 수정해 주세요)
        Vector2 keyboardInput = PlayerInputHandler.Instance.MoveInput;

        dir.x += keyboardInput.x;
        dir.y += keyboardInput.y;

        // 💡 2. InputHandler에서 마우스 위치 받아오기
        if (enableEdgePanning)
        {
            Vector2 mousePos = PlayerInputHandler.Instance.MousePosition;

            if (mousePos.x >= Screen.width - edgePanBorderThickness) dir.x += 1;
            if (mousePos.x <= edgePanBorderThickness) dir.x -= 1;
            if (mousePos.y >= Screen.height - edgePanBorderThickness) dir.y += 1;
            if (mousePos.y <= edgePanBorderThickness) dir.y -= 1;
        }

        return dir;
    }

    private void ClampPosition()
    {
        if (StarSpawnManager.Instance == null || skyCamera == null) return;

        // 💡 수정됨: FakeStarArea가 아니라 패딩이 없는 PanLimitArea를 가져옵니다!
        Rect wideArea = StarSpawnManager.Instance.PanLimitArea;

        float ortho = skyCamera.Lens.OrthographicSize;
        float height = ortho * 2f;
        float width = height * (16f / 9f);

        float minX = wideArea.xMin + (width / 2f);
        float maxX = wideArea.xMax - (width / 2f);
        float minY = wideArea.yMin + (height / 2f);
        float maxY = wideArea.yMax - (height / 2f);

        if (minX > maxX) minX = maxX = wideArea.center.x;
        if (minY > maxY) minY = maxY = wideArea.center.y;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    public void ResetToCenter()
    {
        if (StarSpawnManager.Instance != null)
        {
            // 💡 수정됨: 중앙 복귀도 순수 영역의 정중앙을 기준으로 합니다!
            Vector2 center = StarSpawnManager.Instance.PanLimitArea.center;
            transform.position = new Vector3(center.x, center.y, transform.position.z);
        }
    }
}