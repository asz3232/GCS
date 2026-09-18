using UnityEngine;

/// 인벤토리에 들어가는 아이템 하나의 데이터.
/// 간단한 C# 클래스 방식이라, 아이템 종류가 늘어나면
/// 나중에 ScriptableObject로 옮기기도 쉬움
[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    [TextArea]
    public string description;

    public Item(string itemName, Sprite icon, string description = "")
    {
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
    }
}