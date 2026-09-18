using System;
using UnityEngine;


/// 인벤토리 데이터와 로직을 담당하는 매니저.
/// 슬롯당 아이템 1개, 스택(겹치기) 없음.
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 설정")]
    [SerializeField] private int slotCount = 20;

    // 슬롯 배열. 비어있으면 null.
    private Item[] slots;

    
    /// 인벤토리 내용이 바뀔 때마다 호출됨 (UI 갱신용).
  
    public event Action OnInventoryChanged;

    public int SlotCount => slotCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        slots = new Item[slotCount];
    }


    /// 아이템을 빈 슬롯에 추가합니다. 성공하면 true, 인벤토리가 꽉 차있으면 false.
  
    public bool AddItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

 
    /// 특정 슬롯의 아이템을 제거합니다.

    public void RemoveItemAt(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return;

        slots[slotIndex] = null;
        OnInventoryChanged?.Invoke();
    }


    /// 슬롯 A와 B의 아이템을 서로 바꿉니다 (드래그로 슬롯 이동/정렬할 때 사용).

    public void SwapSlots(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        OnInventoryChanged?.Invoke();
    }

    public Item GetItemAt(int slotIndex)
    {
        if (!IsValidIndex(slotIndex)) return null;
        return slots[slotIndex];
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < slots.Length;
    }
}