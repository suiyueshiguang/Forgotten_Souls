using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerTeleportState : DeathBringerStates
{
    public DeathBringerTeleportState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stats.MakeInvencible(true);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvencible(false);
    }

    public override void Update()
    {
        base.Update();
        
        if(triggerCalled)
        {
            if(enemy.CanDoSpellCast())
            {
                enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.spellCastState);
            }
            else
            {
                enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.battleState);
            }
        }
    }
}
