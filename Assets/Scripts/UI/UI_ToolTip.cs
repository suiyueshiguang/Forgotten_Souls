using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    //注意，这里针对的是 QHD (2560 * 1440)
    [SerializeField] private float xLimit = 1280;
    [SerializeField] private float yLimit = 720;

    [SerializeField] private float xOffset = 300;
    [SerializeField] private float yOffset = 200;

    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        float newXOffset = 0;
        float newYOffset = 0;

        if (mousePosition.x > xLimit)
        {
            newXOffset = -xOffset;
        }
        else
        {
            newXOffset = xOffset;
        }

        if (mousePosition.y > yLimit)
        {
            newYOffset = -yOffset;
        }
        else
        {
            newYOffset = yOffset;
        }

        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }

    public void AdjustFontSize(TextMeshProUGUI _text)
    {
        if (_text.text.Length > 10)
        {
            _text.fontSize = _text.fontSize * .8f;
        }
    }

}
