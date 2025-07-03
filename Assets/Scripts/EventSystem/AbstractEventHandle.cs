using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 输入类型（给我的感觉要不弄成脚本，方便后面的自定义按键的实现）
/// </summary>
public enum InputType
{
    LeftClick
}

public enum EventLevelType
{
    low,
    middle,
    high
}

public abstract class AbstractEventHandle
{
    /// <summary>
    /// 将输入类型和它们各自的输入等级进行保存
    /// </summary>
    private static Dictionary<InputType, Stack<EventLevelType>> eventLevelStack = new Dictionary<InputType, Stack<EventLevelType>>();

    //用来描述事件的一些信息
    private InputType input;
    private EventLevelType level;

    /// <summary>
    /// 构造函数,并将输入类型和输入等级添加到栈里
    /// </summary>
    /// <param name="_input">输入类型</param>
    /// <param name="_level">输入等级</param>
    public AbstractEventHandle(InputType _input, EventLevelType _level)
    {
        this.input = _input;
        this.level = _level;

        if (!eventLevelStack.ContainsKey(_input))
        {
            eventLevelStack[_input] = new Stack<EventLevelType>();
        }

        PushEventLevel(_level);
    }

    /// <summary>
    /// 向栈压入等级事件
    /// </summary>
    /// <param name="_input">事件类型</param>
    /// <param name="_level">事件等级</param>
    private void PushEventLevel(EventLevelType _level)
    {
        if (eventLevelStack[input].Count == 0 || eventLevelStack[input].Peek() < _level)
        {
            eventLevelStack[input].Push(_level);
            return;
        }

        if (eventLevelStack[input].Peek() == _level)
        {
            return;
        }

        //下面是指栈顶元素的值大于所给的_level
        EventLevelType topLevel = eventLevelStack[input].Peek();
        eventLevelStack[input].Pop();
        PushEventLevel(_level);
        PushEventLevel(topLevel);
    }

    /// <summary>
    /// 弹出栈顶元素
    /// </summary>
    /// <param name="_input">输入类型</param>
    public static void PopEventLevel(InputType _input)
    {
        if (eventLevelStack.Count > 0)
        {
            eventLevelStack[_input].Pop();
        }
    }

    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="_type">事件类型</param>
    /// <param name="_action">执行的事件</param>
    public void EventHandle(UnityAction _action)
    {
        if (level < eventLevelStack[input].Peek())
        {
            return;
        }

        _action?.Invoke();
    }
}
