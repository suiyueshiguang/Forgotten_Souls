using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour, ISaveManager, IInventory
{
    public List<ItemData> startingItems;

    //目前装备
    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    //装备库存
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    //材料库存
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("背包UI")]
    [SerializeField] private string inventoryFullPath;
    [SerializeField] private string stashFullPath;
    [SerializeField] private string equipmentFullPath;
    [SerializeField] private string statFullPath;
    private Transform inventorySlotParent;
    private Transform stashSlotParent;
    private Transform equipmentSlotParent;
    private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EuqipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("物品冷却")]
    private float lastTimeUsedFlack;
    private float lastTimeUsedArmor;

    [Header("物品数据库")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private float flaskCooldown;
    private float armorCooldown;

    private bool inCraftState;
    private int deleteAmountInCraft = 1;

    private void Awake()
    {
        if (ServiceLocator.GetService<IInventory>() == null)
        {
            ServiceLocator.Register<IInventory>(this);
        }
    }

    private void Start()
    {
        inventorySlotParent = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(inventoryFullPath);
        stashSlotParent = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(stashFullPath);
        equipmentSlotParent = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(equipmentFullPath);
        statSlotParent = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(statFullPath);

        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EuqipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int index = 0; index < item.stackSize; index++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }

        for (int index = 0; index < startingItems.Count; index++)
        {
            if (startingItems[index] != null)
            {
                AddItem(startingItems[index]);
            }
        }
    }

    /// <summary>
    /// 装备道具
    /// </summary>
    /// <param name="_item">要装备的道具</param>
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;

        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        //遍历当前所穿戴的装备，有无和我们新的同一个种类
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
            }
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);

        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateSlotUI();
    }

    /// <summary>
    /// 取消装备
    /// </summary>
    /// <param name="itemToRemove">要移除的装备</param>
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    /// <summary>
    /// 更新UI界面
    /// </summary>
    private void UpdateSlotUI()
    {
        for (int index = 0; index < equipmentSlot.Length; index++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[index].equipmentType)
                {
                    equipmentSlot[index].UpdateSlot(item.Value);
                }
            }
        }

        //清空背包UI
        for (int index = 0; index < inventoryItemSlot.Length; index++)
        {
            inventoryItemSlot[index].ClearSlot();
        }
        for (int index = 0; index < stashItemSlot.Length; index++)
        {
            stashItemSlot[index].ClearSlot();
        }

        //检查有多少个物品在库存
        for (int index = 0; index < inventory.Count; index++)
        {
            inventoryItemSlot[index].UpdateSlot(inventory[index]);
        }
        for (int index = 0; index < stash.Count; index++)
        {
            stashItemSlot[index].UpdateSlot(stash[index]);
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        if (statSlot == null)
        {
            return;
        }

        for (int index = 0; index < statSlot.Length; index++)
        {
            statSlot[index].UpdateStatValueUI();
        }
    }

    /// <summary>
    /// 添加物品到物品栏
    /// </summary>
    /// <param name="_item">物品</param>
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    /// <summary>
    /// 添加装备到材料层
    /// </summary>
    /// <param name="_item">要添加的材料</param>
    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(1);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    /// <summary>
    /// 添加物品到装备层
    /// </summary>
    /// <param name="_item">添加的装备</param>
    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            //对装备进行累加
            value.AddStack(1);
            //对装备进行单个独立
            //inventory.Add(value);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    /// <summary>
    /// 移除名为_item的物品
    /// </summary>
    /// <param name="_item">_要移除的物品</param>
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack(1);
            }
        }

        else if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
            {
                if (inCraftState)
                {
                    if (stashValue.stackSize <= deleteAmountInCraft)
                    {
                        stash.Remove(stashValue);
                        stashDictionary.Remove(_item);
                    }
                    else
                    {
                        stashValue.RemoveStack(deleteAmountInCraft);
                    }
                }
                else
                {
                    stashValue.RemoveStack(1);
                }
            }
        }

        UpdateSlotUI();
    }

    /// <summary>
    /// 添加物品时判断背包格子是不是满的，以防止出现溢出
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("背包已满");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 创造物品
    /// </summary>
    /// <param name="_itemToCraft">创造的物品</param>
    /// <param name="_requireMaterials">创造物品所需要的材料</param>
    /// <returns>返回能否创造物品</returns>
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requireMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        if (!CanAddItem())
        {
            return false;
        }

        for (int index = 0; index < _requireMaterials.Count; index++)
        {
            if (stashDictionary.TryGetValue(_requireMaterials[index].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requireMaterials[index].stackSize)
                {
                    Debug.Log("没有足够的材料");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("没有相关的材料");
                return false;
            }
        }

        inCraftState = true;
        for (int index = 0; index < materialsToRemove.Count; index++)
        {
            deleteAmountInCraft = _requireMaterials[index].stackSize;
            RemoveItem(materialsToRemove[index].data);
        }
        inCraftState = false;

        AddItem(_itemToCraft);

        Debug.Log("获得道具: " + _itemToCraft.name);
        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetInventoryList() => inventory;

    public float GetFlaskCooldown() => flaskCooldown;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
            }
        }

        return equipedItem;
    }

    /// <summary>
    /// 使用血瓶
    /// </summary>
    public void UseFlack()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
        {
            return;
        }

        bool canUseFlack = Time.time > lastTimeUsedFlack + flaskCooldown;

        if (canUseFlack)
        {
            currentFlask.Effect(null);
            lastTimeUsedFlack = Time.time;
            flaskCooldown = currentFlask.itemCooldown;
        }
        else
        {
            Debug.Log("果粒橙还在冷却");
        }
    }

    /// <summary>
    /// 检测盔甲是否冷却完毕
    /// </summary>
    /// <returns>返回能否使用盔甲/returns>
    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("盔甲正在冷却");
        return false;
    }

    public void LoadData(GameData _data)
    {
        //读取文件中的道具
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            //获取游戏的全部道具
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("建立物品数据库")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);

            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}
