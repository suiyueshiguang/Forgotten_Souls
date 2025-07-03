using UnityEngine;

public class Enemy_NightBorne : Enemy
{
    public NightBorneStateFactory nightBorneStateFactory;

    private float fsxTimer;

    [Header("声效延迟时间")]
    [SerializeField] private float fsxMoveTime;
    [SerializeField] private float fsxBattleTime;

    [Header("炸弹效果")]
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;
    [SerializeField] private float explosiveRadius;


    #region state
    public string idleState { get; private set; } = "Idle";
    public string moveState { get; private set; } = "Move";
    public string battleState { get; private set; } = "Battle";
    public string attackState { get; private set; } = "Attack";
    public string stunnedState { get; private set; } = "Stunned";
    public string deadState { get; private set; } = "Die";
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        nightBorneStateFactory = new NightBorneStateFactory(this, stateMachine);

        nightBorneStateFactory.InitializedNightBorneState();
    }

    protected override void Update()
    {
        base.Update();

        /*
        fsxTimer -= Time.deltaTime;

        if (animator.speed != 0 && fsxTimer < 0)
        {
            NightBorneMoveSFX();
        }
        */
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            nightBorneStateFactory.ChangeNightBorneState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        nightBorneStateFactory.ChangeNightBorneState(deadState);
    }

    private void NightBorneMoveSFX()
    {
        if (nightBorneStateFactory.nightBorneState is NightBorneMoveState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxMoveTime;
        }

        if (nightBorneStateFactory.nightBorneState is NightBorneBattleState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxBattleTime;
        }
    }

    public override void AnimationSpecialAttackTrigger()
    {
        gameObject.GetComponentInChildren<Explosive_Controller>().SetupExplosive(stats, growSpeed, maxSize, explosiveRadius);

        cd.enabled = false;
        rb.gravityScale = 0;
    }
}
