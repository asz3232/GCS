using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    //[SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("시점 회전 설정")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minLookAngle = -80f;
    [SerializeField] private float maxLookAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalLookRotation;
    private bool cursorLocked = true;
    private bool gameplayInputEnabled = true; // 인벤토리 등 UI가 열리면 false로 전환

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        SetCursorLock(true);
    }

    private void Update()
    {
        if (!gameplayInputEnabled) return; // 인벤토리 등 UI가 열려있으면 조작 무시

        HandleCursorToggle();
        HandleMouseLook();
        HandleMovement();
    }

    // 인벤토리 같은 UI가 열리고 닫힐 때 외부(InventoryUI 등)에서 호출합니다.
    // false를 주면 커서가 풀리고 시점 회전/이동이 멈춥니다.
    public void SetGameplayInputEnabled(bool enabled)
    {
        gameplayInputEnabled = enabled;
        SetCursorLock(enabled);
    }

    private void HandleCursorToggle()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null) return;

        // Esc 키로 커서 잠금 해제/재잠금 (UI 확인용)
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            SetCursorLock(!cursorLocked);
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame && !cursorLocked)
        {
            SetCursorLock(true);
        }
    }

    private void SetCursorLock(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void HandleMouseLook()
    {
        if (!cursorLocked || playerCamera == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;

        // 좌우 회전은 몸통(Player) 전체를 회전
        transform.Rotate(Vector3.up * delta.x);

        // 상하 회전은 카메라만 회전 (각도 제한)
        verticalLookRotation -= delta.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            // 살짝 음수로 유지해야 isGrounded가 안정적으로 true를 반환함
            velocity.y = -2f;
        }

        float inputX = 0f;
        float inputZ = 0f;

        if (keyboard.aKey.isPressed) inputX -= 1f;
        if (keyboard.dKey.isPressed) inputX += 1f;
        if (keyboard.sKey.isPressed) inputZ -= 1f;
        if (keyboard.wKey.isPressed) inputZ += 1f;

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        move = Vector3.ClampMagnitude(move, 1f);

        bool isRunning = keyboard.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);
        /*
        // 점프
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        */
        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}