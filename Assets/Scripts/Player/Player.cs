using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("攻击细节")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    [Header("移动信息")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float swordReturnImpact;

    [Header("冲刺信息")]
    public float dashSpeed;
    public float dashDuration;

    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeed;

    public float dashDir { get; private set; }
    public bool isBusy { get; private set; }
    public ISkillManager skill { get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }

    public IPlayerStateMachine stateMachine { get; private set; }

    public PlayerStateFactory playerStateFactory { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();
        playerStateFactory = new PlayerStateFactory(this, stateMachine);
    }

    protected override void Start()
    {
        base.Start();

        skill = ServiceLocator.GetService<ISkillManager>();
        fx = GetComponent<PlayerFX>();

        playerStateFactory.InitializedPlayerState();

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.timeScale == 0)
        {
            return;
        }

        stateMachine.GetCurrentState().Update();
        CheckForDashInput();

        //创建水晶
        if (Input.GetButtonDown("Skill_Crystal") && skill.GetCrystal().crystalUnlocked)
        {
            skill.GetCrystal().CanUseSkill();
        }

        //喝药
        if (Input.GetButtonDown("Flask"))
        {
            ServiceLocator.GetService<IInventory>().UseFlack();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _duration)
    {
        moveSpeed *= (1 - _slowPercentage);
        jumpForce *= (1 - _slowPercentage);
        dashSpeed *= (1 - _slowPercentage);
        animator.speed *= (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _duration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        playerStateFactory.ChangePlayerState(PlayerStates.catchSwordState);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void AnimationTrigger()
    {
        stateMachine.GetCurrentState().AnimationFinishTrigger();
    }

    private void CheckForDashInput()
    {
        if (IsWallDetected())
        {
            return;
        }

        if (!skill.GetDash().dashUnlocked)
        {
            return;
        }

        if (Input.GetButtonDown("Skill_Dash") && ServiceLocator.GetService<ISkillManager>().GetDash().CanUseSkill())
        {
            if (playerStateFactory.playerState is PlayerBlackholeState)
            {
                return;
            }

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            playerStateFactory.ChangePlayerState(PlayerStates.dashState);
        }
    }

    public override void Die()
    {
        base.Die();

        playerStateFactory.ChangePlayerState(PlayerStates.deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
