public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 0.4f;

        player.SetVelocity(5 * -player.facingDir, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.airState);
        }

        if (player.IsGroundDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }
    }
}
