using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform;
    private Slider healthUI;

    /*这里如果有怪物要取消活动，注销此代码，否则请把start的相同部分进行删除
    private void OnEnable()
    {
        entity.onFilpped += FilpUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }
    */

    private void OnDisable()
    {
        if (entity != null)
        {
            entity.onFilpped -= FilpUI;
        }

        if (myStats != null)
        {
            myStats.onHealthChanged -= UpdateHealthUI;
        }
    }

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        healthUI = GetComponentInChildren<Slider>();

        entity.onFilpped += FilpUI;
        myStats.onHealthChanged += UpdateHealthUI;

        ///防止游戏刚启动时，血条为0（存在玩家死亡后保存了血量为小于等于0的记录）
        if(myStats.currentHealth <= 0)
        {
            myStats.currentHealth = myStats.GetMaxHealthValue();
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthUI.maxValue = myStats.GetMaxHealthValue();

        healthUI.value = myStats.currentHealth;
    }

    private void FilpUI()
    {
        myTransform.Rotate(0, 180, 0);

        //为了防止翻转时导致血条z轴从 -9 到 +9 ，导致水面显示出血条
        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, -1 * myTransform.position.z);
    }

}
