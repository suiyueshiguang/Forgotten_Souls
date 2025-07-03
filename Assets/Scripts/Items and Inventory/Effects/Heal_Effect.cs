using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "数据/物品效果/治疗效果")]
public class HealEffect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        float healAmount = playerStats.GetMaxHealthValue() * healPercent;

        playerStats.IncreaseHealthBy(healAmount);
    }
}
