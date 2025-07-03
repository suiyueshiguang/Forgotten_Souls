using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image[] materialImage;

    [SerializeField] private Button craftButton;

    private bool isTooLong;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        //清空制作界面
        for (int index = 0; index < materialImage.Length; index++)
        {
            materialImage[index].color = Color.clear;
            materialImage[index].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        //显示制作界面(长度最多四个)，但可以弄成A要B，B要C，C要D
        for (int index = 0; index < _data.craftingMaterials.Count; index++)
        {
            TextMeshProUGUI materialSlotText = materialImage[index].GetComponentInChildren<TextMeshProUGUI>();

            if (_data.craftingMaterials.Count > materialImage.Length)
            {
                Debug.Log("材料过多，制作槽有限");
            }

            materialImage[index].sprite = _data.craftingMaterials[index].data.icon;
            materialImage[index].color = Color.white;

            materialSlotText.color = Color.white;
            materialSlotText.text = _data.craftingMaterials[index].stackSize.ToString();
        }

        itemIcon.sprite = _data.icon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => ServiceLocator.GetService<IInventory>().CanCraft(_data, _data.craftingMaterials));
    }
}
