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
    /// 设置nightBorne默认状态（游戏默认为idle状态）
    /// </summary>
    public void InitializedNightBorneState()
    {
        nightBorneState = new NightBorneIdleState(enemy, stateMachine, "Idle", enemy);

        stateMachine.Initialize(nightBorneState);
    }

    /// <summary>
    /// 提供改变Enemy_NightBorne的状态的改变
    /// </summary>
    /// <param name="_state">改变的状态</param>
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
