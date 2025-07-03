using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

    private void OnDisable()
    {
        HideToolTip();
    }

    public void ShowToolTip(string _skillDescription, string _skillName, int _cost)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescription;
        skillCost.text = "技能花费:  " + _cost.ToString("#,#");

        //在考虑要用这个自定义方法还是直接用Auto Size
        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }

}
