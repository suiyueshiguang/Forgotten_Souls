using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Íæ¼ÒµôÂä")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    /// <summary>
    /// ÎïÆ·µôÂä
    /// </summary>
    public override void GenerateDrop()
    {
        IInventory inventory = ServiceLocator.GetService<IInventory>();
        
        List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> currentInventory = inventory.GetInventoryList();

        InventoryItem itemsToUnequip = null;
        InventoryItem materialsToLoose = null;

        for (int index = currentEquipment.Count - 1; index >= 0; index--)
        {
            itemsToUnequip = currentEquipment[index];

            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(itemsToUnequip.data);
                inventory.UnequipItem(itemsToUnequip.data as ItemData_Equipment);
            }
        }

        for (int index = currentInventory.Count - 1; index >= 0; index--)
        {
            materialsToLoose = currentInventory[index];

            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(materialsToLoose.data);
                inventory.RemoveItem(materialsToLoose.data);
            }
        }
    }
}

