using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemeis effect", menuName = "数据/物品效果/冻结敌人")]
public class FreezeEnemy_Effect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _transform)
    {
        if (playerStats.currentHealth <= playerStats.GetMaxHealthValue() * 0.1f && ServiceLocator.GetService<IInventory>().CanUseArmor())
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 3);

            foreach (var hit in colliders)
            {
                hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
            }
        }
    }
}
