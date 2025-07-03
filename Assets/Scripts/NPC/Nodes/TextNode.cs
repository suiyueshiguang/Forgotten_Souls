using UnityEngine;

public class TextNode : BaseNode
{
    [SerializeField] private string speaker;
    [TextArea]
    [SerializeField] private string text; 
}
