using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerStunnedState : DeathBringerStates
{
    public DeathBringerStunnedState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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

        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.idleState);
        }
    }
}
