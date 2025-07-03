using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("克隆信息")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float attackMultiplier;
    [Space]

    [Header("残像")]
    [SerializeField] private float cloneAttackMultiplier;
    private UI_SkillTreeSlot cloneAttackUnlockButton;
    public bool canAttack;

    [Header("分身")]
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("多重残影")]
    [SerializeField] private float multipleCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    private UI_SkillTreeSlot multipleUnlockButton;

    [Header("投影")]
    private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInseadOfClone;

    private void Awake()
    {
        cloneAttackUnlockButton = eventSystemManager.GetSubjectTransform(EventType.cloneAttackUnlockButton).GetComponent<UI_SkillTreeSlot>();
        aggresiveCloneUnlockButton = eventSystemManager.GetSubjectTransform(EventType.aggresiveCloneUnlockButton).GetComponent<UI_SkillTreeSlot>();
        multipleUnlockButton = eventSystemManager.GetSubjectTransform(EventType.multipleUnlockButton).GetComponent<UI_SkillTreeSlot>();
        crystalInsteadUnlockButton = eventSystemManager.GetSubjectTransform(EventType.crystalInsteadUnlockButton).GetComponent<UI_SkillTreeSlot>();
    }

    protected override void Start()
    {
        base.Start();

        //订阅服务
        eventSystemManager.Subscribe(EventType.cloneAttackUnlockButton, UnlockCloneAttack);
        eventSystemManager.Subscribe(EventType.aggresiveCloneUnlockButton, UnlockAggresiveClone);
        eventSystemManager.Subscribe(EventType.multipleUnlockButton, UnlockMultiClone);
        eventSystemManager.Subscribe(EventType.crystalInsteadUnlockButton, UnlockCrystalInstead);
    }

    protected override void checkUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }

    #region unlock skill region
    private void UnlockCloneAttack()
    {
        if(cloneAttackUnlockButton.unlocked && !canAttack)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if(aggresiveCloneUnlockButton.unlocked && !canApplyOnHitEffect)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if(multipleUnlockButton.unlocked && !canDuplicateClone)
        {
            canDuplicateClone = true;
            attackMultiplier = multipleCloneAttackMultiplier;
        }    
    }

    private void UnlockCrystalInstead()
    {
        if(crystalInsteadUnlockButton.unlocked && !crystalInseadOfClone)
        {
            crystalInseadOfClone = true;
        }
    }

    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            ServiceLocator.GetService<ISkillManager>().GetCrystal().CreateCrystal();
            ServiceLocator.GetService<ISkillManager>().GetCrystal().CurrentCrystalChooseRandomTarget();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetUpClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        Vector3 offset = new Vector3(Random.Range(0, 2f) * player.facingDir, 0, 0);

        StartCoroutine(CloneDelayCorotine(0.35f, _enemyTransform, offset));
    }

    private IEnumerator CloneDelayCorotine(float seconds, Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(seconds);
        CreateClone(_transform, _offset);
    }
}
