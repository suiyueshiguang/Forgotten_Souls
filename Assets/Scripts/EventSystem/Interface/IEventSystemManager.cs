using UnityEngine;
using UnityEngine.Events;

public enum EventType
{
    //Ͷ��������(���｣��Ӳֱ�����Ƽף����佣���ɽ���������)
    swordUnlockButton,
    timeStopUnlockButton,
    vulnerableUnlockButton,
    bounceUnlockButton,
    pierceUnlockButton,
    spinUnlockButton,

    //��¡����(���񣬷���ͶӰ�����ز���)
    cloneAttackUnlockButton,
    aggresiveCloneUnlockButton,
    crystalInsteadUnlockButton,
    multipleUnlockButton,

    //�ڶ�����
    blackHoleUnlockButton,

    //ˮ������(ˮ����ˮ��˲�ƣ�ˮ����ը�����ÿ��ƣ�����ˮ��)
    unlockCrystalButton,
    unlockCloneInstandButton,
    unlockExplosiveButton,
    unlockMovingCrystalButton,
    unlockMultiStackButton,

    //���ܻػ�����(���ܣ��ػ�)
    unlockDodgeButton,
    unlockMirageDodge,

    //��̼���(��̣������⣬������)
    dashUnlockButton,
    cloneOnDashUnlockButton,
    cloneOnArrivalUnlockButton,

    //�м�(�мܣ��мָܻ������ٷ�Ӧ)
    parryUnlockButton,
    restoreUnlockButton,
    parryWithMirageUnlockButton,

    //����������¼�(���жԻ���������Ϊ)[�����Ⱥ�˳��]
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
