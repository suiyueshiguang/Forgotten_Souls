using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager, IInitializeDisabledObjectData
{
    [SerializeField] private EventType skillType;
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    private UI ui;
    public bool unlocked;
    private Image skillImage;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    public void Initialization()
    {
        IEventSystemManager eventSystemManager = ServiceLocator.GetService<IEventSystemManager>();

        IEventSource eventSource = eventSystemManager.CreateEventSource(GetComponent<Button>().transform);
        eventSystemManager.RegisterEventSource(skillType, eventSource);
        eventSystemManager.Subscribe(skillType, UnlockSkillSlot);
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        skillImage.color = lockedSkillColor;
        ui = GetComponentInParent<UI>();

        if (unlocked)
        {
            skillImage.color = Color.white;
        }
    }

    private void UnlockSkillSlot()
    {
        if (!unlocked && !ServiceLocator.GetService<IPlayerManager>().HaveEnoughMoney(skillCost))
        {
            return;
        }

        //未解锁的技能前面必须全部解锁
        for (int index = 0; index < shouldBeUnlocked.Length; index++)
        {
            if (!shouldBeUnlocked[index].unlocked)
            {
                Debug.Log("无法解锁技能");
                return;
            }
        }

        //存在技能冲突
        for (int index = 0; index < shouldBeLocked.Length; index++)
        {
            if (shouldBeLocked[index].unlocked)
            {
                Debug.Log("无法解锁技能");
                return;
            }
        }

        unlocked = true;

        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription, skillName, skillCost);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
        }

        _data.skillTree.Add(skillName, unlocked);
    }
}
