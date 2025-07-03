using UnityEngine;

public class DeathBringerGroundState : DeathBringerStates
{
    protected Transform playerTransform;

    public DeathBringerGroundState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
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
    }
}
