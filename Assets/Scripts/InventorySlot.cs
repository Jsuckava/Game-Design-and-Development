using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventorySlot : MonoBehaviour
{
    [Header("UI Reference")]
    public Image itemIcon;
    public TextMeshProUGUI countText;
    public Button slotButton;
    
    private Sprite itemSprite;
    private string itemName;
    private string itemDescription;

    private UnityAction<string, string, Sprite> onSlotClicked;
 
    public void Initialize(string name, int count, Sprite icon, string desc, UnityAction<string, string, Sprite> clickCallBack)
    {
        itemName = name;
        itemDescription = desc;
        itemSprite = icon;
        onSlotClicked = clickCallBack;
        if (itemIcon != null) itemIcon.sprite = icon;
        countText.text = count >= 1 ? $"x{count}" : "";
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnClicked);   
        }
    }

    private void OnClicked()
    {
        if (onSlotClicked != null)
        {
            onSlotClicked?.Invoke(itemName, itemDescription, itemSprite);
        }
    }


}
