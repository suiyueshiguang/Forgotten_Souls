using UnityEngine;
using XNodeEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomNodeEditor(typeof(BaseNode))]
public class DialogueEditor : NodeEditor
{
    private SerializedObject nodeObject;
    private SerializedProperty nodeNameProperty;

    /// <summary>
    /// 在设置引用后，创建时初始化
    /// </summary>
    public override void OnCreate()
    {
        base.OnCreate();
        nodeObject = new SerializedObject(target);

        //使用SerializedProperty系统来监控特定节点对象的变化
        nodeNameProperty = nodeObject.FindProperty("nodeName");
    }

    public override void OnHeaderGUI()
    {
        //更新序列化对象
        nodeObject.Update();

        //获取当前的名称
        string nameBuffer = nodeNameProperty.stringValue;

        if(nameBuffer.Equals(""))
        {
            nameBuffer = target.name;
        }

        GUILayout.Label(nameBuffer, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    public override void OnBodyGUI()
    {
        //开始时
        nodeObject.Update();
        string preNameValue = nodeNameProperty.stringValue;

        //绘制节点主体
        base.OnBodyGUI();

        //结束时
        nodeObject.Update();
        string postNameValue = nodeNameProperty.stringValue;

        bool nodeNameChanged = (!preNameValue.Equals(postNameValue));

        //检查是否有任何属性被修改
        if (nodeNameChanged)
        {
            RepainHeader();
        }

        //应用修改
        nodeObject.ApplyModifiedProperties();
    }

    public override int GetWidth()
    {
        return 300;
    }

    private void RepainHeader()
    {
#if UNITY_EDITOR
        if (NodeEditorWindow.current != null)
        {
            NodeEditorWindow.current.Repaint();
        }
#endif
    }
}