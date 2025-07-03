using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        GameObject.Find("Canvas(Clone)").GetComponent<UI>().SwitchOnEndScreen();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
    }
}
