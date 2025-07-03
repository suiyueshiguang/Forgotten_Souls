public class SkeletonStateFactory
{
    private Enemy_Skeleton enemy;
    private IEnemyStateMachine stateMachine;
    public SkeletonStates skeletonState { get; private set; }

    public SkeletonStateFactory(Enemy_Skeleton _enemy, IEnemyStateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// 设置skeleton默认状态（游戏默认为idle状态）
    /// </summary>
    public void InitializedSkeletonState()
    {
        skeletonState = new SkeletonIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(skeletonState);
    }

    /// <summary>
    /// 提供改变Enemy_Skeleton的状态的改变
    /// </summary>
    /// <param name="_state">改变的状态</param>
    public void ChangeSkeletonState(string _state)
    {
        skeletonState = null;

        switch (_state)
        {
            case "Idle":
                {
                    skeletonState = new SkeletonIdleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Move":
                {
                    skeletonState = new SkeletonMoveState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Battle":
                {
                    skeletonState = new SkeletonBattleState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Attack":
                {
                    skeletonState = new SkeletonAttackState(enemy, stateMachine, "Attack", enemy);
                    break;
                }
            case "Stunned":
                {
                    skeletonState = new SkeletonStunnedState(enemy, stateMachine, "Stunned", enemy);
                    break;
                }
            case "Die":
                {
                    skeletonState = new SkeletonDeadState(enemy, stateMachine, "Die", enemy);
                    break;
                }
        }

        stateMachine.ChangeState(skeletonState);
    }
}
