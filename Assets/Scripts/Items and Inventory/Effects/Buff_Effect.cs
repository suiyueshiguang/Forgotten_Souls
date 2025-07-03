using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "数据/物品效果/Buff效果")]
public class Buff_Effect : ItemEffect
{
    [SerializeField] private StatType buffType;
    [SerializeField] private float buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition) => playerStats.IncreaseStatBy(buffAmount, buffDuration, playerStats.GetStat(buffType));
}
