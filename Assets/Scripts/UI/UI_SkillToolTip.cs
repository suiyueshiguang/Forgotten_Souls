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
        skillCost.text = "���ܻ���:  " + _cost.ToString("#,#");

        //�ڿ���Ҫ������Զ��巽������ֱ����Auto Size
        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }

}
