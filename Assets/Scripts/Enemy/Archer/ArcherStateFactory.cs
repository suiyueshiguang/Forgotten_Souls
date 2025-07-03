public class ArcherStateFactory
{
    private Enemy_Archer enemy;
    private IEnemyStateMachine stateMachine;
    public ArcherStates archerState { get; private set; }

    public ArcherStateFactory(Enemy_Archer _enemy, IEnemyStateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// ����archerĬ��״̬����ϷĬ��Ϊidle״̬��
    /// </summary>
    public void InitializedArcherState()
    {
        archerState = new ArcherIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(archerState);
    }

    /// <summary>
    /// �ṩ�ı�Enemy_Archer��״̬�ĸı�
    /// </summary>
    /// <param name="_state">�ı��״̬</param>
    public void ChangeArcherState(string _state)
    {
        archerState = null;

        switch (_state)
        {
            case "Idle":
                {
                    archerState = new ArcherIdleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Move":
                {
                    archerState = new ArcherMoveState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Battle":
                {
                    archerState = new ArcherBattleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Attack":
                {
                    archerState = new ArcherAttackState(enemy, stateMachine, "Attack", enemy);
                    break;
                }
            case "Stunned":
                {
                    archerState = new ArcherStunnedState(enemy, stateMachine, "Stunned", enemy);
                    break;
                }
            case "Die":
                {
                    archerState = new ArcherDeadState(enemy, stateMachine, "Die", enemy);
                    break;
                }
            case "Jump":
                {
                    archerState = new ArcherJumpState(enemy, stateMachine, "Jump", enemy);
                    break;
                }
        }

        stateMachine.ChangeState(archerState);
    }
}
