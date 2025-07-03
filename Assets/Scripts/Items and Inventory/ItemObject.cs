using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody2D rb;

    private void SetupVisuals()
    {
        if (itemData == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.name;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }

    public void PickupItem()
    {
        if (!ServiceLocator.GetService<IInventory>().CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);

            ServiceLocator.GetService<IPlayerManager>().GetPlayer().fx.CreatePopUpText("×°±¸ÒÑÂú");

            return;
        }

        ServiceLocator.GetService<IAudioManager>().PlaySFX("GetItem", transform);
        ServiceLocator.GetService<IInventory>().AddItem(itemData);
        Destroy(gameObject);
    }
}
