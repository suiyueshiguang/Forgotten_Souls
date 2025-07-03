using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public static int comboCounter { get; private set; }

    private static float lastTimeAttacked;
    private float comboWindow = 2f;

    private AbstractEventHandle eventHandle;

    public PlayerPrimaryAttackState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0;

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        player.animator.SetInteger("ComboCounter", comboCounter);
        //加快动画速度
        player.animator.speed = 1f;

        float attackDir = player.facingDir;

        if (xInput != 0)
        {
            attackDir = xInput;
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);
        player.animator.speed = 1f;

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        if (triggerCalled)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }
    }
}
