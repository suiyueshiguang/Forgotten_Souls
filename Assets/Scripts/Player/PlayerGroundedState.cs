using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private AbstractEventHandle attackHandler = new AttackHandler(InputType.LeftClick, EventLevelType.low);

    public PlayerGroundedState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
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

        //左键攻击
        if (Input.GetButtonDown("LeftClick"))
        {
            attackHandler.EventHandle(() => { player.playerStateFactory.ChangePlayerState(PlayerStates.primaryAttackState); });
        }

        //Q反击
        if (Input.GetButtonDown("Skill_Parry") && player.skill.GetParry().parryUnlocked)
        {
            if (player.skill.GetParry().cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("技能还在冷却");
                return;
            }

            player.playerStateFactory.ChangePlayerState(PlayerStates.counterAttackState);
        }

        if (!player.IsGroundDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.airState);
        }

        if (Input.GetButtonDown("Jump") && player.IsGroundDetected())
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.jumpState);
        }

        if (Input.GetButtonDown("Skill_Sword") && HasNoSword() && player.skill.GetSword().swordUnlocked)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.aimSwordState);
        }

        if (Input.GetButtonDown("Skill_BlackHole") && player.skill.GetBlackhole().blackHoleUnlocked)
        {
            if (player.skill.GetBlackhole().cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("技能还在冷却");
                return;
            }

            player.playerStateFactory.ChangePlayerState(PlayerStates.blackholeState);
        }

    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();

        return false;
    }
}
