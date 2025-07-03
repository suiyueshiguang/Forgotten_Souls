using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui => GetComponentInParent<UI>();

    [SerializeField] private string statName;

    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValue;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
    }

    private void Start()
    {
        UpdateStatValueUI();
    }

    /// <summary>
    /// ��ȡ������󣬽���ʾ����
    /// </summary>
    private void OnDisable()
    {
        ui?.statToolTip.HideStartToolTip();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = ServiceLocator.GetService<IPlayerManager>().GetPlayer().GetComponent<PlayerStats>();

        //����Ϊ�ӵ�ʱ����UI����
        if (playerStats != null)
        {
            statValue.text = playerStats.GetStat(statType).GetValue().ToString();

            if (statType == StatType.health)
            {
                statValue.text = playerStats.GetMaxHealthValue().ToString();
            }

            if (statType == StatType.damage)
            {
                statValue.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
            }

            if (statType == StatType.critatePower)
            {
                statValue.text = (playerStats.critatePower.GetValue() + playerStats.strength.GetValue()).ToString();
            }

            if (statType == StatType.critateChance)
            {
                statValue.text = (playerStats.critateChance.GetValue() + playerStats.agility.GetValue()).ToString();
            }

            if (statType == StatType.evasion)
            {
                statValue.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
            }

            if (statType == StatType.magicResistance)
            {
                statValue.text = (playerStats.magicResistance.GetValue() + playerStats.intelligence.GetValue() * 3).ToString();
            }

            /*����Ϊ��ħ�����ﲻӦ��ֱ�Ӽӻ��ױ��˺�����ͨ��ʹ��Ԫ���˺��ٽ������
            */
        }
    }

    /// <summary>
    /// ������ʱ��������
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData) => ui.statToolTip.ShowStartToolTip(statDescription);

    /// <summary>
    /// ����ƿ�ʱ��������
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) => ui.statToolTip.HideStartToolTip();
}
