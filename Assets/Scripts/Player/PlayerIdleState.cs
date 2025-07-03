using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput == player.facingDir && player.IsWallDetected())
        {
            return;
        }

        if (xInput != 0 && !player.isBusy)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.moveState);
        }
    }
}
