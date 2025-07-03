using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerStateFactory
{
    private Enemy_DeathBringer enemy;
    private IEnemyStateMachine stateMachine;
    public DeathBringerStates deathBringerState { get; private set; }

    public DeathBringerStateFactory(Enemy_DeathBringer _enemy, IEnemyStateMachine _stateMachine)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
    }

    /// <summary>
    /// ����deathBringerĬ��״̬����ϷĬ��Ϊidle״̬��
    /// </summary>
    public void InitializedDeathBringerState()
    {
        deathBringerState = new DeathBringerIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(deathBringerState);
    }

    /// <summary>
    /// �ṩ�ı�Enemy_DeathBringer��״̬�ĸı�
    /// </summary>
    /// <param name="_state">�ı��״̬</param>
    public void ChangeDeathBringerState(string _state)
    {
        deathBringerState = null;

        switch (_state)
        {
            case "Idle":
                {
                    deathBringerState = new DeathBringerIdleState(enemy, stateMachine, "Idle", enemy);
                    break;
                }
            case "Teleport":
                {
                    deathBringerState = new DeathBringerTeleportState(enemy, stateMachine, "Teleport", enemy);
                    break;
                }
            case "Battle":
                {
                    deathBringerState = new DeathBringerBattleState(enemy, stateMachine, "Move", enemy);
                    break;
                }
            case "Attack":
                {
                    deathBringerState = new DeathBringerAttackState(enemy, stateMachine, "Attack", enemy);
                    break;
                }
            case "SpellCast":
                {
                    deathBringerState = new DeathBringerSpellCastState(enemy, stateMachine, "SpellCast", enemy);
                    break;
                }
            case "Stunned":
                {
                    deathBringerState = new DeathBringerStunnedState(enemy, stateMachine, "Stunned", enemy);
                    break;
                }
            case "Die":
                {
                    deathBringerState = new DeathBringerDeadState(enemy, stateMachine, "Die", enemy);
                    break;
                }
        }

        stateMachine.ChangeState(deathBringerState);
    }
}
