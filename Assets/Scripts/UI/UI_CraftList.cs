using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;

    void Start()
    {
        //默认打开第一个孩子（制作武器）的界面
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();

        SetupDefaultCraftWindow();
    }

    /// <summary>
    /// 先销毁全部的能创造，再重新设置，确保无多余的(总感觉有性能损失)
    /// </summary>
    public void SetupCraftList()
    {
        for (int index = 0; index < craftSlotParent.childCount; index++)
        {
            Destroy(craftSlotParent.GetChild(index).gameObject);
        }

        for (int index = 0; index < craftEquipment.Count; index++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);

            UI_CraftSlot newSlotComponent = newSlot.GetComponent<UI_CraftSlot>();
            newSlotComponent.SetupCraftSlot(craftEquipment[index]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
        {
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
        }
    }
}
