using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;

    public PlayerCatchSwordState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        if (player.IsGroundDetected())
        {
            player.fx.PlayerDustFX();
        }

        player.fx.ScreenShake(player.fx.shakeSwordImpact);

        //鼠标在角色左边，但角色面向右边
        if (player.transform.position.x > sword.position.x && player.facingDir == 1)
        {
            player.Filp();
        }
        //鼠标在角色右边，但角色面向左边
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1)
        {
            player.Filp();
        }

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .1f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }
    }
}
