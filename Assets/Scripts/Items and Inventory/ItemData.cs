using System.Text;
using UnityEngine;

//因为在游戏中不会有编辑器，所以要限制下面的using
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "数据/物品")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    [Range(0, 100)]
    public float dropChange;

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
        //提供物品在文件中所属的ID,以便后续的保存
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDescription() => "";
}
