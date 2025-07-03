using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("ˮ��")]
    private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("ˮ��˲��")]
    [SerializeField] private bool cloneInsteadOfCrystal;
    private UI_SkillTreeSlot unlockCloneInstandButton;

    [Header("ˮ����ը")]
    [SerializeField] private float explosiveCooldown;
    [SerializeField] private bool canExplode;
    private UI_SkillTreeSlot unlockExplosiveButton;

    [Header("���ÿ���")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool canMoveToEnemy;
    private UI_SkillTreeSlot unlockMovingCrystalButton;

    [Header("����ˮ��")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimerWindow;
    private UI_SkillTreeSlot unlockMultiStackButton;

    //������bug,���л����и��ʳ�bug(destroy��û�������л���ͬ��)
    private List<GameObject> crystalLeft = new List<GameObject>();

    private void Awake()
    {
        unlockCrystalButton = eventSystemManager.GetSubjectTransform(EventType.unlockCrystalButton).GetComponent<UI_SkillTreeSlot>();
        unlockCloneInstandButton = eventSystemManager.GetSubjectTransform(EventType.unlockCloneInstandButton).GetComponent<UI_SkillTreeSlot>();
        unlockExplosiveButton = eventSystemManager.GetSubjectTransform(EventType.unlockExplosiveButton).GetComponent<UI_SkillTreeSlot>();
        unlockMovingCrystalButton = eventSystemManager.GetSubjectTransform(EventType.unlockMovingCrystalButton).GetComponent<UI_SkillTreeSlot>();
        unlockMultiStackButton = eventSystemManager.GetSubjectTransform(EventType.unlockMultiStackButton).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        RefilCrystal();

        eventSystemManager.Subscribe(EventType.unlockCrystalButton, UnlockCrystal);
        eventSystemManager.Subscribe(EventType.unlockCloneInstandButton, UnlockCrystalMirage);
        eventSystemManager.Subscribe(EventType.unlockExplosiveButton, UnlockExplosiveCrystal);
        eventSystemManager.Subscribe(EventType.unlockMovingCrystalButton, UnlockMovingCrytal);
        eventSystemManager.Subscribe(EventType.unlockMultiStackButton, UnlockMultiStack);
    }

    protected override void checkUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrytal();
        UnlockMultiStack();
    }

    /// <summary>
    /// �˴�Ϊ���弼�ܵĽ���
    /// </summary>
    #region Unlock skill region
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked && !crystalUnlocked)
        {
            crystalUnlocked = true;
        }
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInstandButton.unlocked && !cloneInsteadOfCrystal)
        {
            cloneInsteadOfCrystal = true;
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked && !canExplode)
        {
            canExplode = true;
            cooldown = explosiveCooldown;
        }
    }

    private void UnlockMovingCrytal()
    {
        if (unlockMovingCrystalButton.unlocked && !canMoveToEnemy)
        {
            canMoveToEnemy = true;
        }
    }

    private void UnlockMultiStack()
    {
        if (unlockMultiStackButton.unlocked && !canUseMultiStacks)
        {
            canUseMultiStacks = true;
        }
    }

    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
        {
            return;
        }

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
            {
                return;
            }

            //��λ��
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();

            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);

        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimerWindow);
                }

                cooldown = 0;

                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// װ��ˮ��
    /// </summary>
    private void RefilCrystal()
    {
        int amountToAdds = amountOfStacks - crystalLeft.Count;

        for (int index = 0; index < amountToAdds; index++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldown > 0)
        {
            return;
        }

        cooldown = multiStackCooldown;
        RefilCrystal();

    }
}
