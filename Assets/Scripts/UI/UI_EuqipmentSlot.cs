using UnityEngine.EventSystems;

public class UI_EuqipmentSlot : UI_ItemSlot
{
    public EquipmentType equipmentType;

    private void OnValidate()
    {
        if (equipmentType.ToString() == null)
        {
            gameObject.name = "Equipment slot - No data";
        }
        else
        {
            gameObject.name = "Equipment slot - " + equipmentType.ToString();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
        {
            return;
        }

        ServiceLocator.GetService<IInventory>().UnequipItem(item.data as ItemData_Equipment);
        ServiceLocator.GetService<IInventory>().AddItem(item.data as ItemData_Equipment);

        ui.itemToolTip.HideToolTip();

        ClearSlot();
    }
}
