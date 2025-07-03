using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherIdleState : ArcherGroundState
{
    public ArcherIdleState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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

        if (stateTimer < 0f)
        {
            enemy.archerStateFactory.ChangeArcherState(enemy.moveState);
        }
    }
}
