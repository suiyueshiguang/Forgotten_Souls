public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.GetDash().CloneOnDash();

        stateTimer = player.dashDuration;

        player.stats.MakeInvencible(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.GetDash().CloneOnArrival();

        player.SetVelocity(0, rb.velocity.y);

        player.stats.MakeInvencible(false);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.wallSlideState);
        }

        if (stateTimer < 0)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }

        if (!player.skill.GetDash().cloneDashUnlocked)
        {
            player.fx.CreateAfterImage();
        }
    }
}
