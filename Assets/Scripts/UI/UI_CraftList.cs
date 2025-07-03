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
        //Ĭ�ϴ򿪵�һ�����ӣ������������Ľ���
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();

        SetupDefaultCraftWindow();
    }

    /// <summary>
    /// ������ȫ�����ܴ��죬���������ã�ȷ���޶����(�ܸо���������ʧ)
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
