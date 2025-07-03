using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    public void ShowToolTip(ItemData_Equipment _item)
    {
        if(_item == null)
        {
            return;
        }

        itemNameText.text = _item.itemName;
        itemTypeText.text = ShowItemTypeTextToChinese(_item);
        itemDescription.text = _item.GetDescription();

        AdjustFontSize(itemTypeText);
        AdjustPosition();

        gameObject.SetActive(true);
    }

    private string ShowItemTypeTextToChinese(ItemData_Equipment _item)
    {
        switch (_item.equipmentType.ToString())
        {
            case "Weapon":
                return "ÎäÆ÷";
            case "Armor":
                return "·À¾ß";
            case "Amulet":
                return "»¤·û";
            case "Flask":
                return "Ò©Ë®";
            default:
                return "";
        }
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
