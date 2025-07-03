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
    /// ��ȡ������󣬽���ʾ����
    /// </summary>
    private void OnDisable()
    {
        ui?.itemToolTip.HideToolTip();
    }

    /// <summary>
    /// ������Ʒ��
    /// </summary>
    /// <param name="_newItem">��Ʒ�۵�λ��</param>
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
    /// ��յ�ǰ��Ʒ�۵�������ʾ��Ϣ
    /// </summary>
    public void ClearSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;

        itemText.text = "";
    }

    /// <summary>
    /// �����ʱ��������
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        //������Ʒ
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
    /// ������ʱ��������
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
    /// ����ƿ�ʱ��������
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
