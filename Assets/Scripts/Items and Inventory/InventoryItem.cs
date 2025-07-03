using System;
using System.Diagnostics;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;

        AddStack(1);
    }

    public void AddStack(int _addStackSize) => stackSize += _addStackSize;

    public void RemoveStack(int _removeStackSize) => stackSize -= _removeStackSize;
}
