using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventSystemManager : MonoBehaviour, IEventSystemManager
{
    //一个事件类型只能对应一个事件源
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
    /// 绑定事件源（注意：一个事件类型只能有一个事件源）
    /// </summary>
    public void RegisterEventSource(EventType _eventType, IEventSource _eventSource)
    {
        if (eventSources.ContainsKey(_eventType))
        {
            Debug.Log($"事件类型为:{_eventType}已经被绑定了事件源，现在进行重新更新");
        }

        eventSources[_eventType] = _eventSource;
    }

    /// <summary>
    /// 创建事件源
    /// </summary>
    /// <param name="_transformType">事件类型</param>
    /// <returns>返回事件源</returns>
    public IEventSource CreateEventSource(Transform _transformType)
    {
        if (_transformType.TryGetComponent<Button>(out var button))
        {
            return new ButtonEventSource(button);
        }

        return new GameObjectEventSource(_transformType.gameObject);
    }

    /// <summary>
    /// 订阅服务
    /// </summary>
    /// <param name="_type">监视事件类型</param>
    /// <param name="_handler">服务</param>
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
    /// 注销服务
    /// </summary>
    /// <param name="_type">监视事件类型</param>
    /// <param name="_handler">服务</param>
    public void Unsubscribe(EventType _type, UnityAction _handler)
    {
        if (eventSources.TryGetValue(_type, out var source))
        {
            source.RemoveListener(_handler);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="_type">触发事件的类型</param>
    public void RaiseEvent(EventType _type)
    {
        if (eventSources.TryGetValue(_type, out var source))
        {
            source.Invoke();
        }
        else
        {
            Debug.LogError("没有注册事件源");
        }
    }

    /// <summary>
    /// 获取被观察者的Transform
    /// </summary>
    /// <param name="_eventType">被观察者的类型</param>
    /// <returns>被观察者的Transform</returns>
    public Transform GetSubjectTransform(EventType _eventType)
    {
        if (eventSources.TryGetValue(_eventType, out var eventSource))
        {
            return eventSource.subjectTransform;
        }

        return null;
    }
}
