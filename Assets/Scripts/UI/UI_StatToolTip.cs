using TMPro;
using UnityEngine;

public class UI_StatToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI description;

    public void ShowStartToolTip(string _text)
    {
        description.text = _text;

        AdjustPosition();

        gameObject.SetActive(true);
    }

    public void HideStartToolTip()
    {
        description.text = "";

        gameObject.SetActive(false);
    }
}
