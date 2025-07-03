using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "����/��ƷЧ��/BuffЧ��")]
public class Buff_Effect : ItemEffect
{
    [SerializeField] private StatType buffType;
    [SerializeField] private float buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition) => playerStats.IncreaseStatBy(buffAmount, buffDuration, playerStats.GetStat(buffType));
}
