public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        ServiceLocator.GetService<IAudioManager>().StopSFX("PlayerMove_Leaves_1");
    }

    public override void Update()
    {
        base.Update();

        if (sfxTimer < 0)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX("PlayerMove_Leaves_1", null);
            sfxTimer = 0.4f;
        }

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (xInput == 0 || player.IsWallDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }

    }
}
