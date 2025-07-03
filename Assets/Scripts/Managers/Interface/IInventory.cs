using System.Collections.Generic;

public interface IInventory
{
    public void EquipItem(ItemData _item);
    public void UnequipItem(ItemData_Equipment itemToRemove);
    public void UpdateStatsUI();
    public void AddItem(ItemData _item);
    public void RemoveItem(ItemData _item);
    public bool CanAddItem();
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requireMaterials);
    public List<InventoryItem> GetEquipmentList();
    public List<InventoryItem> GetInventoryList();
    public ItemData_Equipment GetEquipment(EquipmentType _type);
    public void UseFlack();
    public bool CanUseArmor();
    public float GetFlaskCooldown();
}
