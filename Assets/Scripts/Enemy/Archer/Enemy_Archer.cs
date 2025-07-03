using UnityEngine;

public class Enemy_Archer : Enemy
{
    public ArcherStateFactory archerStateFactory;

    private float fsxTimer;

    [Header("声效延迟时间")]
    [SerializeField] private float fsxMoveTime;
    [SerializeField] private float fsxBattleTime;

    [Header("弓箭手信息")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    public Vector2 jumpVelocity;
    public float jumpCooldown;
    public float safeDistance;      //离玩家距离多近才出发后撤跳
    [HideInInspector] public float lastTimeJumped;

    [Header("检查后面")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    #region state
    public string idleState { get; private set; } = "Idle";
    public string moveState { get; private set; } = "Move";
    public string battleState { get; private set; } = "Battle";
    public string attackState { get; private set; } = "Attack";
    public string stunnedState { get; private set; } = "Stunned";
    public string deadState { get; private set; } = "Die";
    public string jumpState { get; private set; } = "Jump";
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        archerStateFactory = new ArcherStateFactory(this, stateMachine);

        archerStateFactory.InitializedArcherState();
    }

    protected override void Update()
    {
        base.Update();

        /*
        fsxTimer -= Time.deltaTime;

        if (animator.speed != 0 && fsxTimer < 0)
        {
            SkeletonMoveSFX();
        }
        */
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            archerStateFactory.ChangeArcherState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        archerStateFactory.ChangeArcherState(deadState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.transform.position, Quaternion.identity);

        newArrow.GetComponent<Arrow_Controller>().SetupArrow(arrowSpeed * facingDir, stats);

    }

    private void ArcherMoveSFX()
    {
        if (archerStateFactory.archerState is ArcherMoveState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxMoveTime;
        }

        if (archerStateFactory.archerState is ArcherBattleState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxBattleTime;
        }
    }

    public bool GroundBehindCheck() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);

    public bool WallBehind() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}
