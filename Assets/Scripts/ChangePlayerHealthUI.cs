using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerHealthUI : MonoBehaviour
{
    [SerializeField] private string playerUIHealthBarFullPath;
    private Toggle toggle;
    private GameObject healthBar;

    private void Awake()
    {
        healthBar = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(playerUIHealthBarFullPath).gameObject;

        toggle = GetComponent<Toggle>();

        //Í¬²½×´Ì¬
        toggle.isOn = healthBar.activeSelf;

        toggle.onValueChanged.AddListener(ChangePlayerUIHealthBar);
        ChangePlayerUIHealthBar(healthBar.activeSelf);
    }

    private void ChangePlayerUIHealthBar(bool _value) => healthBar.SetActive(_value);
}
