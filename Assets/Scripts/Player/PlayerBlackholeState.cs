using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        stateTimer = flyTime;
        skillUsed = false;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransprent(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            player.SetVelocity(0, 15);
        }
        if (stateTimer < 0)
        {
            player.SetVelocity(0, -0.1f);

            if (!skillUsed)
            {
                if (player.skill.GetBlackhole().CanUseSkill())
                { 
                    skillUsed = true; 
                }
            }
        }

       if(player.skill.GetBlackhole().SkillCompleted())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.airState);
        }
    }
}
