using UnityEngine;
using UnityEngine.InputSystem;


/// 플레이어 카메라가 바라보는 방향으로 레이캐스트를 쏴서
/// WorldItem을 감지하고, 상호작용 키를 누르면 줍습니다.
///
/// 사용법:
/// 1) Player(또는 카메라를 가진 오브젝트)에 이 스크립트를 붙입니다.
/// 2) Interact Camera 필드에 FirstPersonController에서 쓰던 Main Camera를 연결합니다.
/// 3) Interact Layer는 아이템 오브젝트들이 속한 레이어로 설정하면
///    성능/오탐 방지에 좋습니다 (선택, 기본은 All).

public class ItemInteractor : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private Transform interactCamera;

    [Header("상호작용 설정")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer = ~0; // 기본: 모든 레이어

    private WorldItem currentTarget;

    private void Update()
    {
        DetectTarget();
        HandleInteractInput();
    }


    /// 매 프레임 카메라 정면으로 레이캐스트를 쏴서 WorldItem을 찾습니다.

    private void DetectTarget()
    {
        currentTarget = null;

        if (interactCamera == null) return;

        Ray ray = new Ray(interactCamera.position, interactCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent<WorldItem>(out WorldItem worldItem))
            {
                currentTarget = worldItem;
            }
        }

        // TODO: 여기서 currentTarget != null 여부에 따라
        // "F 눌러서 줍기" 같은 UI 프롬프트를 켜고 끄면 됩니다.
    }

    private void HandleInteractInput()
    {
        if (currentTarget == null) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            currentTarget.Interact();
        }
    }
}