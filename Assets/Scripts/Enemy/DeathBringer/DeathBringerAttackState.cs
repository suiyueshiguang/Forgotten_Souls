using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerAttackState : DeathBringerStates
{
    public DeathBringerAttackState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.chanceToTeleport += 5f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
        {
            if (enemy.CanTeleport())
            {
                enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.teleportState);
            }
            else
            {
                enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.battleState);
            }
        }
    }
}
