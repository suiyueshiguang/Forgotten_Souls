using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    //���Ƶ������¡����
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

        //�뾶 +5 ��Ϊ�˵����˹�����Χ�������ʱ�ҹ������ʱ���ܹ�������Ч��
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

                    //���ڼ���(�мܻظ�)(���������PlayerGroundState����ʾ������ȴ�ж�ε���)
                    player.skill.GetParry().CanUseSkill();

                    //���ڼ���(���ٻػ�)
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
