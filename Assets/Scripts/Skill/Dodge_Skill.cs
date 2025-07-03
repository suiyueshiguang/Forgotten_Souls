using UnityEngine;

public class Dodge_Skill : Skill
{
    [Header("����")]
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked;
    private UI_SkillTreeSlot unlockDodgeButton;

    [Header("�ػ�")]
    public bool dodgeMirageUnlocked;
    private UI_SkillTreeSlot unlockMirageDodge;

    private void Awake()
    {
        unlockDodgeButton = eventSystemManager.GetSubjectTransform(EventType.unlockDodgeButton).GetComponent<UI_SkillTreeSlot>();
        unlockMirageDodge = eventSystemManager.GetSubjectTransform(EventType.unlockMirageDodge).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        eventSystemManager.Subscribe(EventType.unlockDodgeButton, UnlockDodge);
        eventSystemManager.Subscribe(EventType.unlockMirageDodge, UnlockMirageDodge);
    }

    protected override void checkUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    #region unlock skill region
    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            //��������bug,���������player��stats����start˳���ͻ(���������ִ���ˣ�player.stats������û����)
            player.stats.evasion.AddModifier(10);
            ServiceLocator.GetService<IInventory>().UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if (unlockMirageDodge.unlocked && !dodgeMirageUnlocked)
        {
            dodgeMirageUnlocked = true;
        }
    }
    #endregion

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
        {
            ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(player.transform, new Vector3(player.attackCheckRadius * 1.25f, 0));
        }
    }
}
