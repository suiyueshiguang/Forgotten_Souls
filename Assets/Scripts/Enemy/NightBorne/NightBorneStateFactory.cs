using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneStateFactory
{
    private Enemy_NightBorne enemy;
    private IEnemyStateMachine stateMachine;
    public NightBorneStates nightBorneState { get; private set; }

    public NightBorneStateFactory(Enemy_NightBorne _enemy, IEnemyStateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// ����nightBorneĬ��״̬����ϷĬ��Ϊidle״̬��
    /// </summary>
    public void InitializedNightBorneState()
    {
        nightBorneState = new NightBorneIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(nightBorneState);
    }

    /// <summary>
    /// �ṩ�ı�Enemy_NightBorne��״̬�ĸı�
    /// </summary>
    /// <param name="_state">�ı��״̬</param>
    public void ChangeNightBorneState(string _state)
    {
        nightBorneState = null;

        switch (_state)
        {
            case "Idle":
                {
                    nightBorneState = new NightBorneIdleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Move":
                {
                    nightBorneState = new NightBorneMoveState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Battle":
                {
                    nightBorneState = new NightBorneBattleState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Attack":
                {
                    nightBorneState = new NightBorneAttackState(enemy, stateMachine, "Attack", enemy);
                    break;
                }
            case "Stunned":
                {
                    nightBorneState = new NightBorneStunnedState(enemy, stateMachine, "Stunned", enemy);
                    break;
                }
            case "Die":
                {
                    nightBorneState = new NightBorneDeadState(enemy, stateMachine, "Die", enemy);
                    break;
                }
        }

        stateMachine.ChangeState(nightBorneState);
    }
}
