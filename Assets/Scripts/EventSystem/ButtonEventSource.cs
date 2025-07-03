using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 按钮事件源
/// </summary>
public class ButtonEventSource : IEventSource
{
    private Button button;

    public Transform subjectTransform => button.transform;

    public ButtonEventSource(Button _button)
    {
        button = _button;
    }

    public void AddListener(UnityAction _action)
    {
        button.onClick.AddListener(_action);
    }

    public void RemoveListener(UnityAction _action)
    {
        button.onClick.RemoveListener(_action);
    }

    public void Invoke()
    {
    }
}
