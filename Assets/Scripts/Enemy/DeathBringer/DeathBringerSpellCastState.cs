using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DeathBringerSpellCastState : DeathBringerStates
{
    private int amountOfSpells;
    private float spellTimer;

    public DeathBringerSpellCastState(Enemy _enemyBase, IEnemyStateMachine _StateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _StateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        amountOfSpells = enemy.amountOfSpells;
        spellTimer = .5f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeCast = Time.time;
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        if(CanCast())
        {
            enemy.CanSpell();
        }

        else if(amountOfSpells <= 0)
        {
            enemy.deathBringerStateFactory.ChangeDeathBringerState(enemy.teleportState);
        }
    }

    private bool CanCast()
    {
        if(amountOfSpells > 0 && spellTimer < 0)
        {
            spellTimer = enemy.spellCooldown;
            amountOfSpells--;
            return true;
        }

        return false;
    }
}
