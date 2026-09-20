using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// 인벤토리에서 아이템을 클릭했을 때 아이콘 + 이름 + 설명을 보여주는 패널.
/// 바이오하자드처럼 슬롯을 누르면 화면에 상세 정보가 뜨는 용도입니다.

public class ItemDetailPanel : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private GameObject panelRoot; // 보통 이 스크립트가 붙은 오브젝트 자신
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;

    [Header("닫기")]
    [SerializeField] private Button closeButton; // 별도 X 버튼 (선택)
    [SerializeField] private Button backdropButton; // 화면 전체를 덮는 배경, 클릭하면 닫힘 (선택)

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        if (backdropButton != null)
        {
            backdropButton.onClick.AddListener(Hide);
        }

        Hide();
    }

  
    /// 아이템 정보를 채워서 패널을 켭니다.

    public void Show(Item item)
    {
        if (item == null) return;

        if (iconImage != null)
        {
            iconImage.enabled = item.icon != null;
            iconImage.sprite = item.icon;
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = item.description;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }
}