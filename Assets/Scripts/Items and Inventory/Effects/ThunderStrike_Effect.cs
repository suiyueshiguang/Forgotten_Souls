using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "数据/物品效果/雷属性")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);

        Destroy(newThunderStrike, 1f);
    }
}
