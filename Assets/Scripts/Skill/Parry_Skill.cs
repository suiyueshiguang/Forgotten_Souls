using UnityEngine;

public class Parry_Skill : Skill
{
    [Header("招架")]
    private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("招架回复")]
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked { get; private set; }
    private UI_SkillTreeSlot restoreUnlockButton;

    [Header("快速反应")]
    private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    private void Awake()
    {
        parryUnlockButton = eventSystemManager.GetSubjectTransform(EventType.parryUnlockButton).GetComponent<UI_SkillTreeSlot>();
        restoreUnlockButton = eventSystemManager.GetSubjectTransform(EventType.restoreUnlockButton).GetComponent<UI_SkillTreeSlot>();
        parryWithMirageUnlockButton = eventSystemManager.GetSubjectTransform(EventType.parryWithMirageUnlockButton).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        eventSystemManager.Subscribe(EventType.parryUnlockButton, UnlockParry);
        eventSystemManager.Subscribe(EventType.restoreUnlockButton, UnlockParryRestore);
        eventSystemManager.Subscribe(EventType.parryWithMirageUnlockButton, UnlockParryWithMirage);
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            float restoreAmount = player.stats.GetMaxHealthValue() * restoreHealthPercentage;

            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void checkUnlock()
    {
        base.checkUnlock();

        UnlockParry();
        UnlockParryRestore();
        UnlockParryWithMirage();
    }

    #region unlock skill region
    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked && !parryUnlocked)
        {
            parryUnlocked = true;
        }
    }

    private void UnlockParryRestore()
    {
        if (restoreUnlockButton.unlocked && !restoreUnlocked)
        {
            restoreUnlocked = true;
        }
    }

    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked && !parryWithMirageUnlocked)
        {
            parryWithMirageUnlocked = true;
        }
    }
    #endregion

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
        {
            ServiceLocator.GetService<ISkillManager>().GetClone().CreateCloneWithDelay(_respawnTransform);
        }
    }
}
