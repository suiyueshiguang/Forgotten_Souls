using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("冲刺")]
    private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked { get; private set; }

    [Header("我在这")]
    private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneDashUnlocked { get; private set; }

    [Header("我在哪")]
    private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    public bool cloneOnArrivalUnlocked { get; private set; }

    private void Awake()
    {
        dashUnlockButton = eventSystemManager.GetSubjectTransform(EventType.dashUnlockButton).GetComponent<UI_SkillTreeSlot>();
        cloneOnDashUnlockButton = eventSystemManager.GetSubjectTransform(EventType.cloneOnDashUnlockButton).GetComponent<UI_SkillTreeSlot>();
        cloneOnArrivalUnlockButton = eventSystemManager.GetSubjectTransform(EventType.cloneOnArrivalUnlockButton).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        eventSystemManager.Subscribe(EventType.dashUnlockButton, UnlockDash);
        eventSystemManager.Subscribe(EventType.cloneOnDashUnlockButton, UnlockCloneOnDash);
        eventSystemManager.Subscribe(EventType.cloneOnArrivalUnlockButton, UnlockCloneOnArrival);
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void checkUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }

    #region unlock skill region
    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked && !dashUnlocked)
        {
            dashUnlocked = true;
        }
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked && !cloneDashUnlocked)
        {
            cloneDashUnlocked = true;
        }
    }

    private void UnlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked && !cloneOnArrivalUnlocked)
        {
            cloneOnArrivalUnlocked = true;
        }
    }
    #endregion

    public void CloneOnDash()
    {
        if (cloneDashUnlocked)
        {
            ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
        {
            ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(player.transform, Vector3.zero);
        }
    }
}
