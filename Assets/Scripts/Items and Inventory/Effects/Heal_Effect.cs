using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "����/��ƷЧ��/����Ч��")]
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
