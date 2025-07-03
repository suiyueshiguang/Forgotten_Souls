using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for (int index = 0; index < possibleDrop.Length; index++)
        {
            if (Random.Range(0, 100) <= possibleDrop[index].dropChange)
            {
                dropList.Add(possibleDrop[index]);
            }
        }

        for (int index = 0; index < possibleItemDrop && dropList.Count > 0; index++)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        //物品跳出的速度方向
        Vector2 randomVelocity = new Vector2(Random.Range(0,5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
