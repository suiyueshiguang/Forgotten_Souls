using UnityEngine;

public class SkeletonDeadState : SkeletonStates
{
    public SkeletonDeadState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        /*此方法的死亡形式是类似与马里奥的死亡，直接掉出屏幕
        enemy.animator.SetBool(enemy.lastAnimBoolName , true);
        enemy.animator.speed = 0;
        enemy.cd.enabled = false;
        */

        stateTimer = 0.1f;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        /*和上面搭配用，让死亡动画播放一段时间后，掉出屏幕
           enemy.SetVelocity(0, 10);
        */

        //此有极小概率的bug,存在死亡后，怪物并没有触发Dead的动画参数，而是回到Idle状态(改了后并无发现，可能是运气太差了没遇到)
        if (stateTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
        }
    }
}
