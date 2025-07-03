using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class BaseNode : Node
{
    //节点唯一标识
    [SerializeField] private string GUID = System.Guid.NewGuid().ToString();
    [SerializeField] private string nodeName;
    [Input]
    [SerializeField] private List<Empty> input;
    [Output]
    [SerializeField] private List<Empty> output;

    private List<NodePort> dynamicPorts = new List<NodePort>();

    // 动态端口管理
    protected void AddDynamicPort(string fieldName, NodePort.IO direction)
    {
        var port = AddDynamicOutput(typeof(Empty), fieldName: fieldName);
        dynamicPorts.Add(port);
    }

    protected void ClearAllDynamicPorts()
    {
        foreach (var port in dynamicPorts)
        {
            RemoveDynamicPort(port);
        }
        dynamicPorts.Clear();
    }

    // 在请求时返回输出端口的正确值
    public override object GetValue(NodePort _port)
    {
        return null;
    }
}