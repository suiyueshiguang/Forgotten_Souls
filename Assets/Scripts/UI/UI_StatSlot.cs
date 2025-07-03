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
    /// 当取消激活后，将提示隐藏
    /// </summary>
    private void OnDisable()
    {
        ui?.statToolTip.HideStartToolTip();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = ServiceLocator.GetService<IPlayerManager>().GetPlayer().GetComponent<PlayerStats>();

        //下面为加点时进行UI更新
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

            /*我认为在魔法那里不应该直接加火雷冰伤害，在通过使用元素伤害再进行添加
            */
        }
    }

    /// <summary>
    /// 鼠标进入时触发内容
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData) => ui.statToolTip.ShowStartToolTip(statDescription);

    /// <summary>
    /// 鼠标移开时触发内容
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) => ui.statToolTip.HideStartToolTip();
}
