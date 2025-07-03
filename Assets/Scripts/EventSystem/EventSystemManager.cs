using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventSystemManager : MonoBehaviour, IEventSystemManager
{
    //һ���¼�����ֻ�ܶ�Ӧһ���¼�Դ
    private Dictionary<EventType, IEventSource> eventSources;

    private void Awake()
    {
        if (ServiceLocator.GetService<IEventSystemManager>() == null)
        {
            ServiceLocator.Register<IEventSystemManager>(this);
        }

        eventSources = new Dictionary<EventType, IEventSource>();
    }

    /// <summary>
    /// ���¼�Դ��ע�⣺һ���¼�����ֻ����һ���¼�Դ��
    /// </summary>
    public void RegisterEventSource(EventType _eventType, IEventSource _eventSource)
    {
        if (eventSources.ContainsKey(_eventType))
        {
            Debug.Log($"�¼�����Ϊ:{_eventType}�Ѿ��������¼�Դ�����ڽ������¸���");
        }

        eventSources[_eventType] = _eventSource;
    }

    /// <summary>
    /// �����¼�Դ
    /// </summary>
    /// <param name="_transformType">�¼�����</param>
    /// <returns>�����¼�Դ</returns>
    public IEventSource CreateEventSource(Transform _transformType)
    {
        if (_transformType.TryGetComponent<Button>(out var button))
        {
            return new ButtonEventSource(button);
        }

        return new GameObjectEventSource(_transformType.gameObject);
    }

    /// <summary>
    /// ���ķ���
    /// </summary>
    /// <param name="_type">�����¼�����</param>
    /// <param name="_handler">����</param>
    public void Subscribe(EventType _type, UnityAction _handler)
    {
        if (!eventSources.TryGetValue(_type, out var source))
        {
            eventSources[_type] = source;
            return;
        }

        source.AddListener(_handler);
    }

    /// <summary>
    /// ע������
    /// </summary>
    /// <param name="_type">�����¼�����</param>
    /// <param name="_handler">����</param>
    public void Unsubscribe(EventType _type, UnityAction _handler)
    {
        if (eventSources.TryGetValue(_type, out var source))
        {
            source.RemoveListener(_handler);
        }
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="_type">�����¼�������</param>
    public void RaiseEvent(EventType _type)
    {
        if (eventSources.TryGetValue(_type, out var source))
        {
            source.Invoke();
        }
        else
        {
            Debug.LogError("û��ע���¼�Դ");
        }
    }

    /// <summary>
    /// ��ȡ���۲��ߵ�Transform
    /// </summary>
    /// <param name="_eventType">���۲��ߵ�����</param>
    /// <returns>���۲��ߵ�Transform</returns>
    public Transform GetSubjectTransform(EventType _eventType)
    {
        if (eventSources.TryGetValue(_eventType, out var eventSource))
        {
            return eventSource.subjectTransform;
        }

        return null;
    }
}
