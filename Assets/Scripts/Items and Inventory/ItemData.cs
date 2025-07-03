using System.Text;
using UnityEngine;

//��Ϊ����Ϸ�в����б༭��������Ҫ���������using
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "����/��Ʒ")]
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
        //�ṩ��Ʒ���ļ���������ID,�Ա�����ı���
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDescription() => "";
}
