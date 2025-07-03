using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform;
    private Slider healthUI;

    /*��������й���Ҫȡ�����ע���˴��룬�������start����ͬ���ֽ���ɾ��
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

        ///��ֹ��Ϸ������ʱ��Ѫ��Ϊ0��������������󱣴���Ѫ��ΪС�ڵ���0�ļ�¼��
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

        //Ϊ�˷�ֹ��תʱ����Ѫ��z��� -9 �� +9 ������ˮ����ʾ��Ѫ��
        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, -1 * myTransform.position.z);
    }

}
