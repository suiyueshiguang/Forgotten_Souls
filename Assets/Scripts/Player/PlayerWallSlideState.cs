using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(!player.IsWallDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.airState);
        }

        if(Input.GetButtonDown("Jump"))
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.wallJumpState);
            return;
        }

        if(xInput != 0 && player.facingDir != xInput)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }

        if(yInput < 0)
        {
            rb.velocity = new Vector2(0,rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y * 0.7f);
        }

        if (player.IsGroundDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }
    }
}
