using TMPro;
using UnityEngine;

public class UI_DisplayUrged : MonoBehaviour
{
    [SerializeField] private string urgentBoxFullPath;
    [TextArea(1, 2)]
    [SerializeField] private string text;

    private RectTransform urgentBox;
    private TextMeshProUGUI urgentText;

    private bool displayUrgentBox;
    private bool isPlayerInExtent;

    private void Start()
    {
        urgentBox = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(urgentBoxFullPath);

        urgentText = urgentBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isPlayerInExtent && Input.GetButtonDown("Interactive"))
        {
            urgentText.text = text;

            displayUrgentBox = !displayUrgentBox;
            urgentBox.gameObject.SetActive(displayUrgentBox);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInExtent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInExtent = false;

            urgentBox.gameObject.SetActive(false);
        }
    }
}
