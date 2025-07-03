using UnityEngine;

public class SkeletonGroundState : SkeletonStates
{
    protected Transform playerTransform;

    public SkeletonGroundState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerTransform = ServiceLocator.GetService<IPlayerManager>().GetPlayer().transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, playerTransform.position) < enemy.growDistance)
        {
            enemy.skeletonStateFactory.ChangeSkeletonState(enemy.battleState);
        }
    }
}
