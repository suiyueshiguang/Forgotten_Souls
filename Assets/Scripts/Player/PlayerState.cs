using UnityEngine;

public enum PlayerStates
{
    idleState,
    moveState,
    jumpState,
    airState,
    dashState,
    wallSlideState,
    wallJumpState,
    primaryAttackState,
    counterAttackState,
    catchSwordState,
    aimSwordState,
    blackholeState,
    deadState
}

public class PlayerState
{
    protected IPlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;
    private string animBoolName;

    protected float stateTimer;
    protected float sfxTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.animator.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        stateTimer -= Time.deltaTime;
        sfxTimer -= Time.deltaTime;

        player.animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        player.animator.SetBool(animBoolName, false);
    }

    //用来表示一个动画的结束
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
