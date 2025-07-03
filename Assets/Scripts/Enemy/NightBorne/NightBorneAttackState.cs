using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneAttackState : NightBorneStates
{
    public NightBorneAttackState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_NightBorne _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
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
            enemy.nightBorneStateFactory.ChangeNightBorneState(enemy.battleState);
        }
    }
}
