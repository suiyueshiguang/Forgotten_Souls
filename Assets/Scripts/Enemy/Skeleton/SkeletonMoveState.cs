using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{
    public SkeletonMoveState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Filp();
            enemy.skeletonStateFactory.ChangeSkeletonState(enemy.idleState);
        }
    }
}
