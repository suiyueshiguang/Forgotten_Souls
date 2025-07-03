using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected IEnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    protected GameObject gameObject;
    protected SpriteRenderer sr;
    protected float colorLoosingSpeed = 0.3f;

    public EnemyState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName)
    {
        enemyBase = _enemyBase;
        stateMachine = _StateMachine;
        animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;

        rb = enemyBase.rb;
        sr = enemyBase.sr;
        gameObject = enemyBase.gameObject;

        enemyBase.animator.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        enemyBase.animator.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimBool(animBoolName);
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}