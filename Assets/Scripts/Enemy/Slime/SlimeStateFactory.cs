using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStateFactory
{
    private Enemy_Slime enemy;
    private IEnemyStateMachine stateMachine;
    public SlimeStates slimeState { get; private set; }

    public SlimeStateFactory(Enemy_Slime _enemy, IEnemyStateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// ����slimeĬ��״̬����ϷĬ��Ϊidle״̬��
    /// </summary>
    public void InitializedSlimeState()
    {
        slimeState = new SlimeIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(slimeState);
    }

    /// <summary>
    /// �ṩ�ı�Enemy_Slime��״̬�ĸı�
    /// </summary>
    /// <param name="_state">�ı��״̬</param>
    public void ChangeSlimeState(string _state)
    {
        slimeState = null;

        switch (_state)
        {
            case "Idle":
                {
                    slimeState = new SlimeIdleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Move":
                {
                    slimeState = new SlimeMoveState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Battle":
                {
                    slimeState = new SlimeBattleState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Attack":
                {
                    slimeState = new SlimeAttackState(enemy, stateMachine, "Attack", enemy);
                    break;
                }
            case "Stunned":
                {
                    slimeState = new SlimeStunnedState(enemy, stateMachine, "Stunned", enemy);
                    break;
                }
            case "Die":
                {
                    slimeState = new SlimeDeadState(enemy, stateMachine, "Die", enemy);
                    break;
                }
        }

        stateMachine.ChangeState(slimeState);
    }
}
