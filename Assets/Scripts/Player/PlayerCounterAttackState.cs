using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    //限制弹反后克隆次数
    private bool canCreateClone = true;

    public PlayerCounterAttackState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.counterAttackDuration;
        player.animator.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        //半径 +5 是为了当敌人攻击范围大于玩家时且攻击玩家时，能够可以有效格挡
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius + 5);

        foreach (var hit in colliders)
        {
            if(hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccessfulCounterAttack();
            }

            if (hit.GetComponent<Enemy>() != null)
            {
                if(hit.GetComponent<Enemy>().CanBeStunned())
                {
                    SuccessfulCounterAttack();

                    //用于技能(招架回复)(不过这个和PlayerGroundState的显示技能冷却有多次调用)
                    player.skill.GetParry().CanUseSkill();

                    //用于技能(快速回击)
                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.GetParry().MakeMirageOnParry(player.transform);
                    }

                }
            }
        }

        if(stateTimer < 0 || triggerCalled)
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }
    }

    private void SuccessfulCounterAttack()
    {
        stateTimer = 10;
        player.animator.SetBool("SuccessfulCounterAttack", true);
    }
}
