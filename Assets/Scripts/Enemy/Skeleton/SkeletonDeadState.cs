using UnityEngine;

public class SkeletonDeadState : SkeletonStates
{
    public SkeletonDeadState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        /*�˷�����������ʽ������������µ�������ֱ�ӵ�����Ļ
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

        /*����������ã���������������һ��ʱ��󣬵�����Ļ
           enemy.SetVelocity(0, 10);
        */

        //���м�С���ʵ�bug,���������󣬹��ﲢû�д���Dead�Ķ������������ǻص�Idle״̬(���˺��޷��֣�����������̫����û����)
        if (stateTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
        }
    }
}
