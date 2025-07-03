using System.Collections.Generic;

public class ChoiceNode : BaseNode
{
    [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, dynamicPortList = true)]
    public List<string> choices;
}