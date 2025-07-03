using UnityEngine;
using UnityEngine.Events;

public interface IEventSource
{
    public Transform subjectTransform { get; }

    public void AddListener(UnityAction _action);
    public void RemoveListener(UnityAction _action);
    public void Invoke();
}
