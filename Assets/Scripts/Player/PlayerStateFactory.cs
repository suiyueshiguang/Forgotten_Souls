public class PlayerStateFactory
{
    private Player player;
    private IPlayerStateMachine stateMachine;

    public PlayerState playerState { get; private set; }

    public PlayerStateFactory(Player _player, IPlayerStateMachine _stateMachine)
    {
        player = _player;   
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// �������Ĭ��״̬����ϷĬ��Ϊidle״̬��
    /// </summary>
    public void InitializedPlayerState()
    {
        playerState = new PlayerIdleState(player, stateMachine, "Idle");

        stateMachine.Initialized(playerState);
    }

    /// <summary>
    /// �ṩ�ı�Player״̬�ĺ���
    /// </summary>
    /// <param name="_state">�ı�״̬����</param>
    public void ChangePlayerState(PlayerStates _state)
    {
        playerState = null;

        switch (_state)
        {
            case PlayerStates.idleState:
                {
                    playerState = new PlayerIdleState(player, stateMachine, "Idle");
                    break;
                }
            case PlayerStates.moveState:
                {
                    playerState = new PlayerMoveState(player, stateMachine, "Move");
                    break;
                }
            case PlayerStates.jumpState:
                {
                    playerState = new PlayerJumpState(player, stateMachine, "Jump");
                    break;
                }
            case PlayerStates.airState:
                {
                    playerState = new PlayerAirState(player, stateMachine, "Jump");
                    break;
                }
            case PlayerStates.dashState:
                {
                    playerState = new PlayerDashState(player, stateMachine, "Dash");
                    break;
                }
            case PlayerStates.wallSlideState:
                {
                    playerState = new PlayerWallSlideState(player, stateMachine, "WallSlide");
                    break;
                }
            case PlayerStates.wallJumpState:
                {
                    playerState = new PlayerWallJumpState(player, stateMachine, "Jump");
                    break;
                }
            case PlayerStates.primaryAttackState:
                {
                    playerState = new PlayerPrimaryAttackState(player, stateMachine, "Attack");
                    break;
                }
            case PlayerStates.counterAttackState:
                {
                    playerState = new PlayerCounterAttackState(player, stateMachine, "CounterAttack");
                    break;
                }
            case PlayerStates.catchSwordState:
                {
                    playerState = new PlayerCatchSwordState(player, stateMachine, "CatchSword");
                    break;
                }
            case PlayerStates.aimSwordState:
                {
                    playerState = new PlayerAimSwordState(player, stateMachine, "AimSword");
                    break;
                }
            case PlayerStates.blackholeState:
                {
                    playerState = new PlayerBlackholeState(player, stateMachine, "Jump");
                    break;
                }
            case PlayerStates.deadState:
                {
                    playerState = new PlayerDeadState(player, stateMachine, "Die");
                    break;
                }
        }

        stateMachine.ChangeState(playerState);
    }
}
