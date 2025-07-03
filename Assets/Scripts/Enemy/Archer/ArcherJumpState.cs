using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherJumpState : ArcherStates
{
    public ArcherJumpState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, enemy.jumpVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.animator.SetFloat("yVelocity", rb.velocity.y);

        if(rb.velocity.y < 0 && enemy.IsGroundDetected())
        {
            enemy.archerStateFactory.ChangeArcherState(enemy.battleState);
        }
    }
}
