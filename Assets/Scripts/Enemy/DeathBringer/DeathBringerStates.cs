using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerStates : EnemyState
{
    protected Enemy_DeathBringer enemy;

    public DeathBringerStates(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
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
    }
}
