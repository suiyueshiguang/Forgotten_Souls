using UnityEngine;

public class ArcherAttackState : ArcherStates
{
    public ArcherAttackState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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

        if (triggerCalled)
        {
            enemy.archerStateFactory.ChangeArcherState(enemy.battleState);
        }
    }
}
