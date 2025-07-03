using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStunnedState : SlimeStates
{
    public SlimeStunnedState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        stateTimer = enemy.stunDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvencible(false);
    }

    public override void Update()
    {
        base.Update();

        if(rb.velocity.y < .1f && enemy.IsGroundDetected())
        {
            enemy.fx.Invoke("CancelColorChange", 0);
            enemy.animator.SetTrigger("StunFold");
            enemy.stats.MakeInvencible(true);
        }

        if (stateTimer < 0)
        {
            enemy.slimeStateFactory.ChangeSlimeState(enemy.idleState);
        }
    }
}
