using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("被动技能")]
    private UI_SkillTreeSlot timeStopUnlockButton;
    private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    public bool vulnerableUnlocked { get; private set; }

    [Header("抛物剑")]
    private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity = 4f;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("飞剑")]
    [SerializeField] private float pierceGravity;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float launchForceVelocityY;
    private UI_SkillTreeSlot pierceUnlockButton;

    [Header("弹射剑")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;
    private UI_SkillTreeSlot bounceUnlockButton;

    [Header("回旋剑")]
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTraveDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;
    private UI_SkillTreeSlot spinUnlockButton;

    private Vector2 finalDir;

    [Header("瞄准信息")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    private Transform dotsParent;
    private GameObject[] dots;

    private void Awake()
    {
        timeStopUnlockButton = eventSystemManager.GetSubjectTransform(EventType.timeStopUnlockButton).GetComponent<UI_SkillTreeSlot>();
        vulnerableUnlockButton = eventSystemManager.GetSubjectTransform(EventType.vulnerableUnlockButton).GetComponent<UI_SkillTreeSlot>();
        swordUnlockButton = eventSystemManager.GetSubjectTransform(EventType.swordUnlockButton).GetComponent<UI_SkillTreeSlot>();
        pierceUnlockButton = eventSystemManager.GetSubjectTransform(EventType.pierceUnlockButton).GetComponent<UI_SkillTreeSlot>();
        bounceUnlockButton = eventSystemManager.GetSubjectTransform(EventType.bounceUnlockButton).GetComponent<UI_SkillTreeSlot>();
        spinUnlockButton = eventSystemManager.GetSubjectTransform(EventType.spinUnlockButton).GetComponent<UI_SkillTreeSlot>();

        dotsParent = player.transform.Find("AimDotParent");
    }

    protected override void Start()
    {
        base.Start();

        GenereateDots();

        eventSystemManager.Subscribe(EventType.timeStopUnlockButton, UnlockTimeStop);
        eventSystemManager.Subscribe(EventType.vulnerableUnlockButton, UnlockVolunerable);
        eventSystemManager.Subscribe(EventType.swordUnlockButton, UnlockSword);
        eventSystemManager.Subscribe(EventType.bounceUnlockButton, UnlockBounceSword);
        eventSystemManager.Subscribe(EventType.pierceUnlockButton, UnlockPierceSword);
        eventSystemManager.Subscribe(EventType.spinUnlockButton, UnlockSpinSword);
    }

    protected override void Update()
    {
        if (Input.GetButtonUp("Skill_Sword"))
        {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }

        if (Input.GetButton("Skill_Sword"))
        {
            if (pierceUnlockButton.unlocked || spinUnlockButton.unlocked)
            {
                launchForce = new Vector2(launchForce.x, launchForceVelocityY);
            }

            for (int index = 0; index < numberOfDots; index++)
            {
                dots[index].transform.position = DotsPosition(index * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScrip = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
        {
            newSwordScrip.SetupBounce(true, bounceAmount, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            newSwordScrip.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            newSwordScrip.SetUpSpin(true, maxTraveDistance, spinDuration, hitCooldown);
        }

        newSwordScrip.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    protected override void checkUnlock()
    {
        UnlockTimeStop();
        UnlockVolunerable();
        UnlockSword();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
    }

    #region Unlock skill region
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked && !timeStopUnlocked)
        {
            timeStopUnlocked = true;
        }
    }

    private void UnlockVolunerable()
    {
        if (vulnerableUnlockButton.unlocked && !vulnerableUnlocked)
        {
            vulnerableUnlocked = true;
        }
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked && !swordUnlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked && swordType != SwordType.Bounce)
        {
            swordType = SwordType.Bounce;
            swordGravity = bounceGravity;
        }
    }

    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked && swordType != SwordType.Pierce)
        {
            swordType = SwordType.Pierce;
            swordGravity = pierceGravity;
        }
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked && swordType != SwordType.Spin)
        {
            swordType = SwordType.Spin;
            swordGravity = spinGravity;
        }
    }
    #endregion

    #region Aim region

    /// <summary>
    /// 返回鼠标和玩家之间的距离
    /// </summary>
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int index = 0; index < numberOfDots; index++)
        {
            dots[index] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[index].SetActive(false);
        }
    }

    public void DotsActive(bool _isActive)
    {
        for (int index = 0; index < numberOfDots; index++)
        {
            dots[index].SetActive(_isActive);
        }
    }

    private Vector2 DotsPosition(float _t)
    {
        Vector2 position = (Vector2)player.transform.position +
                            AimDirection().normalized * launchForce * _t +
                            .5f * (Physics2D.gravity * swordGravity) * (_t * _t);
        return position;
    }
    #endregion
}
