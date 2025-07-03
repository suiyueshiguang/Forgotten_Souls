using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// �������ͣ����ҵĸо�Ҫ��Ū�ɽű������������Զ��尴����ʵ�֣�
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
    /// ���������ͺ����Ǹ��Ե�����ȼ����б���
    /// </summary>
    private static Dictionary<InputType, Stack<EventLevelType>> eventLevelStack = new Dictionary<InputType, Stack<EventLevelType>>();

    //���������¼���һЩ��Ϣ
    private InputType input;
    private EventLevelType level;

    /// <summary>
    /// ���캯��,�����������ͺ�����ȼ���ӵ�ջ��
    /// </summary>
    /// <param name="_input">��������</param>
    /// <param name="_level">����ȼ�</param>
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
    /// ��ջѹ��ȼ��¼�
    /// </summary>
    /// <param name="_input">�¼�����</param>
    /// <param name="_level">�¼��ȼ�</param>
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

        //������ָջ��Ԫ�ص�ֵ����������_level
        EventLevelType topLevel = eventLevelStack[input].Peek();
        eventLevelStack[input].Pop();
        PushEventLevel(_level);
        PushEventLevel(topLevel);
    }

    /// <summary>
    /// ����ջ��Ԫ��
    /// </summary>
    /// <param name="_input">��������</param>
    public static void PopEventLevel(InputType _input)
    {
        if (eventLevelStack.Count > 0)
        {
            eventLevelStack[_input].Pop();
        }
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="_type">�¼�����</param>
    /// <param name="_action">ִ�е��¼�</param>
    public void EventHandle(UnityAction _action)
    {
        if (level < eventLevelStack[input].Peek())
        {
            return;
        }

        _action?.Invoke();
    }
}
