using UnityEngine;
using UnityEngine.InputSystem;


/// InventoryManager의 데이터를 받아서 슬롯 UI 그리드를 생성/갱신합니다.

public class InventoryUI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotParent; // Grid Layout Group이 붙은 오브젝트
    [SerializeField] private ItemDetailPanel detailPanel;// 클릭 시 설명을 보여줄 패널
    [SerializeField] private FirstPersonController playerController; // 인벤토리 열릴 때 조작/커서 잠금 해제용

    [Header("옵션")]
    [SerializeField] private bool hideOnStartIfClosed = false;
    [SerializeField] private GameObject panelRoot; // 인벤토리 전체 패널 (토글용, 선택)
    [SerializeField] private Key toggleKey = Key.Tab; // 새 Input System 기준

    private InventorySlotUI[] slotUIs;
    private bool isOpen = true;

    private void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("씬에 InventoryManager가 없습니다.");
            return;
        }

        BuildSlots();
        RefreshUI();

        InventoryManager.Instance.OnInventoryChanged += RefreshUI;

        if (panelRoot != null && hideOnStartIfClosed)
        {
            SetOpen(false);
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    private void Update()
    {
        if (panelRoot == null) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            SetOpen(!isOpen);
        }
    }


    /// 슬롯 개수만큼 슬롯 UI를 미리 생성해둡니다 (한 번만 실행).

    private void BuildSlots()
    {
        int count = InventoryManager.Instance.SlotCount;
        slotUIs = new InventorySlotUI[count];

        // 기존 자식이 있으면 정리
        for (int i = slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(slotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < count; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = slot;
        }
    }


    /// InventoryManager 데이터를 기준으로 모든 슬롯 UI를 다시 그립니다.

    private void RefreshUI()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            Item item = InventoryManager.Instance.GetItemAt(i);
            slotUIs[i].Setup(i, item, this);
        }
    }


    /// 슬롯 클릭 시 InventorySlotUI에서 호출됩니다.
    /// 아이템이 있으면 상세 정보 패널(아이콘/이름/설명)을 띄웁니다.
    public void OnSlotClicked(int slotIndex)
    {
        Item item = InventoryManager.Instance.GetItemAt(slotIndex);
        if (item == null) return;

        if (detailPanel != null)
        {
            detailPanel.Show(item);
        }
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (panelRoot != null)
        {
            panelRoot.SetActive(open);
        }

        // 인벤토리가 열리면 플레이어 조작/커서 잠금을 끄고, 닫히면 다시 켭니다.
        if (playerController != null)
        {
            playerController.SetGameplayInputEnabled(!open);
        }
    }


}