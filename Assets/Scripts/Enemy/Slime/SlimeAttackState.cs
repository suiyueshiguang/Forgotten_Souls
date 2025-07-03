using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : SlimeStates
{
    public SlimeAttackState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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
            enemy.slimeStateFactory.ChangeSlimeState(enemy.battleState);
        }
    }
}
