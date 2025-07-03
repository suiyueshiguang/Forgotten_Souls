using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui => GetComponentInParent<UI>();
    public InventoryItem item;

    protected virtual void Start()
    {
    }

    /// <summary>
    /// 当取消激活后，将提示隐藏
    /// </summary>
    private void OnDisable()
    {
        ui?.itemToolTip.HideToolTip();
    }

    /// <summary>
    /// 更新物品槽
    /// </summary>
    /// <param name="_newItem">物品槽的位置</param>
    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else if (item.stackSize == 1)
            {
                itemText.text = "";
            }
            else
            {
                ClearSlot();
            }
        }
    }

    /// <summary>
    /// 清空当前物品槽的所有显示消息
    /// </summary>
    public void ClearSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;

        itemText.text = "";
    }

    /// <summary>
    /// 鼠标点击时触发内容
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        //丢弃物品
        if (Input.GetButton("RemoveItem"))
        {
            ServiceLocator.GetService<IInventory>().RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equipment)
        {
            ServiceLocator.GetService<IInventory>().EquipItem(item.data);
        }

        ui.itemToolTip.HideToolTip();
    }

    /// <summary>
    /// 鼠标进入时触发内容
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    /// <summary>
    /// 鼠标移开时触发内容
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        ui.itemToolTip.HideToolTip();
    }
}
