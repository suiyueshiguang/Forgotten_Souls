using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        ServiceLocator.GetService<IAudioManager>().PlaySFX("SwordSwing", null);

        //下面的代码经常会用到，要不封装成函数或类（建议）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                {
                    if (_target.isDead)
                    {
                        continue;
                    }

                    player.stats.DoDamage(_target);
                }

                ServiceLocator.GetService<IInventory>()?.GetEquipment(EquipmentType.Weapon)?.Effect(_target.transform);

                /*
                //下面为当角色近战偷袭时敌人会转身
                if(player.facingDir == hit.GetComponent<Enemy>().facingDir)
                {
                    hit.GetComponent<Enemy>().Filp();
                }
                */
            }
        }
    }

    private void ThrowSword()
    {
        ServiceLocator.GetService<ISkillManager>().GetSword().CreateSword();
    }
}
