using UnityEngine;

public class Blackhole_Skill : Skill
{
    [Header("ºÚ¶´")]
    private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked { get; private set; }

    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [Space]
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float CloneCooldown;
    [SerializeField] private float blackholeDuration;

    private Blackhole_Skill_Controller currentBlackhole;

    private void UnlockBlackhole()
    {
        if (blackHoleUnlockButton.unlocked && !blackHoleUnlocked)
        {
            blackHoleUnlocked = true;
        }
    }

    public void UpdateBlackholeUnlocked() => blackHoleUnlocked = true;

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, CloneCooldown, blackholeDuration);

        ServiceLocator.GetService<IAudioManager>().PlaySFX(skillSoundName, player.transform);
    }

    private void Awake()
    {
        blackHoleUnlockButton = eventSystemManager.GetSubjectTransform(EventType.blackHoleUnlockButton).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        eventSystemManager.Subscribe(EventType.blackHoleUnlockButton, UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
        {
            return false;
        }

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

    protected override void checkUnlock()
    {
        UnlockBlackhole();
    }
}
