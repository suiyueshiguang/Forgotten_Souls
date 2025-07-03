using UnityEngine;
using UnityEngine.Events;

public enum EventType
{
    //投掷剑技能(抛物剑，硬直化，破甲，弹射剑，飞剑，回旋剑)
    swordUnlockButton,
    timeStopUnlockButton,
    vulnerableUnlockButton,
    bounceUnlockButton,
    pierceUnlockButton,
    spinUnlockButton,

    //克隆技能(残像，分身，投影，多重残像)
    cloneAttackUnlockButton,
    aggresiveCloneUnlockButton,
    crystalInsteadUnlockButton,
    multipleUnlockButton,

    //黑洞技能
    blackHoleUnlockButton,

    //水晶技能(水晶，水晶瞬移，水晶爆炸，更好控制，更多水晶)
    unlockCrystalButton,
    unlockCloneInstandButton,
    unlockExplosiveButton,
    unlockMovingCrystalButton,
    unlockMultiStackButton,

    //闪避回击技能(闪避，回击)
    unlockDodgeButton,
    unlockMirageDodge,

    //冲刺技能(冲刺，我在这，我在哪)
    dashUnlockButton,
    cloneOnDashUnlockButton,
    cloneOnArrivalUnlockButton,

    //招架(招架，招架恢复，快速反应)
    parryUnlockButton,
    restoreUnlockButton,
    parryWithMirageUnlockButton,

    //鼠标左键点击事件(进行对话，攻击行为)[存在先后顺序]
    dialogueHandler,
    attackHandler,
}

[System.Serializable]
public class EventData
{
    public EventType type;
    public string objectName;
    [System.NonSerialized] public Transform transform;
    [System.NonSerialized] public IEventSource eventSource;
}

[System.Serializable]
public struct SubjectData
{
    public string pathPrefix;
    public EventData[] datas;
}

public interface IEventSystemManager
{
    public void RegisterEventSource(EventType _eventType, IEventSource _eventSource);
    public IEventSource CreateEventSource(Transform _transformType);
    public void Subscribe(EventType _type, UnityAction _handler);
    public void Unsubscribe(EventType _type, UnityAction _handler);
    public void RaiseEvent(EventType _type);
    public Transform GetSubjectTransform(EventType _eventType);
}
