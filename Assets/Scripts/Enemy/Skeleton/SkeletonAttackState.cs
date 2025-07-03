using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SkeletonAttackState : SkeletonStates
{
    public SkeletonAttackState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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

        if(triggerCalled)
        {
            enemy.skeletonStateFactory.ChangeSkeletonState(enemy.battleState);
        }
    }
}
