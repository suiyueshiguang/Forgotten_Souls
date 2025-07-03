using UnityEngine;

public class SkeletonBattleState : SkeletonStates
{
    private Transform player;
    private int moveDir;

    public SkeletonBattleState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player = ServiceLocator.GetService<IPlayerManager>().GetPlayer()?.transform;

        if (player!= null && player.GetComponent<PlayerStats>().isDead)
        {
            enemy.skeletonStateFactory.ChangeSkeletonState(enemy.moveState);
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
                    enemy.skeletonStateFactory.ChangeSkeletonState(enemy.attackState);
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
                enemy.skeletonStateFactory.ChangeSkeletonState(enemy.idleState);
            }

            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
            {
                enemy.skeletonStateFactory.ChangeSkeletonState(enemy.moveState);
            }
        }

        moveDir = (player.position.x > enemy.transform.position.x) ? 1 : -1;

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
