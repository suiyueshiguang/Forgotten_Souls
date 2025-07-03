using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 自定义游戏对象事件源
/// </summary>
public class GameObjectEventSource : IEventSource
{
    private event UnityAction Action;
    private GameObject targetGameObject;

    public Transform subjectTransform => targetGameObject.transform;

    public GameObjectEventSource(GameObject _targetGameObject)
    {
        targetGameObject = _targetGameObject;

        var trigger = targetGameObject.GetComponent<EventTrigger>() ?? targetGameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };

        entry.callback.AddListener(_ => Action?.Invoke());

        trigger.triggers.Add(entry);
    }

    public void AddListener(UnityAction _action)
    {
        Action += _action;
    }

    public void RemoveListener(UnityAction _action)
    {
        Action -= _action;
    }

    public void Invoke()
    {
        Action?.Invoke();
    }
}
