using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerIdleState : DeathBringerGroundState
{

    public DeathBringerIdleState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if(Vector2.Distance(playerTransform.position, enemy.transform.position) < 10)
        {
            enemy.bossFightBegun = true;
        }

        if(stateTimer < 0 && enemy.bossFightBegun)
        {
            enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.battleState);
        }
    }
}
