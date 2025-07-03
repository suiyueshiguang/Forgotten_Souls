using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    public SkeletonStateFactory skeletonStateFactory;

    private float fsxTimer;

    [Header("…˘–ß—”≥Ÿ ±º‰")]
    [SerializeField] private float fsxMoveTime;
    [SerializeField] private float fsxBattleTime;

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

        skeletonStateFactory = new SkeletonStateFactory(this, stateMachine);

        skeletonStateFactory.InitializedSkeletonState();
    }

    protected override void Update()
    {
        base.Update();

        fsxTimer -= Time.deltaTime;

        if (animator.speed != 0 && fsxTimer < 0)
        {
            SkeletonMoveSFX();
        }
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            skeletonStateFactory.ChangeSkeletonState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        skeletonStateFactory.ChangeSkeletonState(deadState);
    }

    private void SkeletonMoveSFX()
    {
        if (skeletonStateFactory.skeletonState is SkeletonMoveState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxMoveTime;
        }

        if (skeletonStateFactory.skeletonState is SkeletonBattleState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxBattleTime;
        }
    }
}
