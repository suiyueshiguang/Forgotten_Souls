using UnityEngine;

public class DeathBringerBattleState : DeathBringerStates
{
    private Transform playerTransform;
    private int moveDir;

    public DeathBringerBattleState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerTransform = ServiceLocator.GetService<IPlayerManager>().GetPlayer()?.transform;

        if (playerTransform.GetComponent<PlayerStats>().isDead)
        {
            enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.idleState);
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

            if (enemy.IsPlayerDetected().distance <= enemy.attackDistance)
            {
                if (CanAttack())
                {
                    enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.attackState);
                }
                else
                {
                    enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.idleState);
                }
            }
        }
        else
        {
            if (playerTransform == null)
            {
                return;
            }

            if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
            {
                enemy.Filp();
                enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.idleState);
            }
        }

        moveDir = (playerTransform.position.x > enemy.transform.position.x) ? 1 : -1;

        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - 0.5f)
        {
            return;
        }

        enemy.SetVelocity(enemy.moveSpeed * enemy.battleMoveSpeedMultiply * moveDir, rb.velocity.y);
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
}
