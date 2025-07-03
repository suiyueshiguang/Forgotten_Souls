using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBattleState : ArcherStates
{
    private Transform player;
    private int moveDir;

    public ArcherBattleState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player = ServiceLocator.GetService<IPlayerManager>().GetPlayer()?.transform;

        if (player != null && player.GetComponent<PlayerStats>().isDead)
        {
            enemy.archerStateFactory.ChangeArcherState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.safeDistance)
            {
                if (CanJump())
                {
                    enemy.archerStateFactory.ChangeArcherState(enemy.jumpState);
                }
            }

            if (enemy.IsPlayerDetected().distance <= enemy.attackDistance)
            {
                if (CanAttack())
                {
                    enemy.archerStateFactory.ChangeArcherState(enemy.attackState);
                }
            }
        }
        else
        {
            if (player == null)
            {
                return;
            }

            if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
            {
                enemy.Filp();
                enemy.archerStateFactory.ChangeArcherState(enemy.idleState);
            }

            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
            {
                enemy.archerStateFactory.ChangeArcherState(enemy.moveState);
            }
        }

        BattleStateFlipControl();
    }

    private void BattleStateFlipControl()
    {
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
        {
            enemy.Filp();
        }
        if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
        {
            enemy.Filp();
        }
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.AttackCooldown)
        {
            enemy.AttackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);

            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

    private bool CanJump()
    {
        if(!enemy.GroundBehindCheck() || enemy.WallBehind())
        {
            return false;
        }

        if(Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {
            enemy.lastTimeJumped = Time.time;
            return true;
        }    
        return false;
    }
}
