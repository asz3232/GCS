using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// 인벤토리 그리드의 슬롯 하나를 담당하는 UI.
/// 아이콘 표시 + 클릭 시 선택 이벤트를 InventoryUI로 전달
///
/// 프리팹 구조 (예시):
/// Slot (이 스크립트 + Image 배경 + Button 또는 클릭 처리용 컴포넌트)
///   └ Icon (Image, 아이템 아이콘 표시용, 빈 슬롯일 땐 비활성화)

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color filledColor = Color.white;

    private int slotIndex;
    private InventoryUI ownerUI;


    /// InventoryUI가 슬롯을 생성/갱신할 때 호출

    public void Setup(int index, Item item, InventoryUI owner)
    {
        slotIndex = index;
        ownerUI = owner;
        SetItem(item);
    }

    public void SetItem(Item item)
    {
        bool hasItem = item != null;

        if (iconImage != null)
        {
            iconImage.enabled = hasItem;
            iconImage.sprite = hasItem ? item.icon : null;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = hasItem ? filledColor : emptyColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ownerUI?.OnSlotClicked(slotIndex);
    }
}