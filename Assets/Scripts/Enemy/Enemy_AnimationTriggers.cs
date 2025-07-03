using UnityEngine;

public class Enemy_AnimationTriggers : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    private void AnimationFinishTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        ServiceLocator.GetService<IAudioManager>().PlaySFX("AxeSwing", null);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();

                enemy.stats.DoDamage(target);

                //当敌人的最终造成的魔法伤害大于0，允许敌人对玩家造成魔法伤害
                if (ApplyDoMagicDamage(target))
                {
                    enemy.stats.DoMagicDamage(target);
                }
            }
        }
    }

    private void SpecialAttackTrigger()
    {
        enemy.AnimationSpecialAttackTrigger();
    }

    private bool ApplyDoMagicDamage(CharacterStats _targetStats)
    {
        CharacterStats enemyStats = enemy.stats;

        float totalMagicalDamage = enemyStats.fireDamage.GetValue() + enemyStats.iceDamage.GetValue() + enemyStats.lightningDamage.GetValue() + enemy.stats.intelligence.GetValue();

        totalMagicalDamage = enemyStats.CheckTargetResistance(_targetStats, totalMagicalDamage);

        return totalMagicalDamage > 0;
    }

    protected void OpenCounterWindow() => enemy.OpenCounterAttackWindow();

    protected void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
