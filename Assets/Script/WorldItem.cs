using UnityEngine;


public class WorldItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;

    /// 이 오브젝트가 나타내는 Item 데이터를 반환합니다.

    public Item GetItemData()
    {
        return new Item(itemName, icon, description);
    }

    
    public bool Interact()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager가 씬에 없습니다.");
            return false;
        }

        bool added = InventoryManager.Instance.AddItem(GetItemData());
        if (added)
        {
            Debug.Log($"{itemName} 획득");
            Destroy(gameObject);
            //gameObject.SetActive(false); // 필요하면 Destroy(gameObject)로 바꿔도 됨
        }
        else
        {
            Debug.Log("인벤토리가 가득 차서 주울 수 없습니다.");
        }

        return added;
    }
}