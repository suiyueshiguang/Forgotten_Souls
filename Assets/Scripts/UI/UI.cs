using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("��������")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;
    public UI_SkillToolTip skillToolTip;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        //��ǰҪ�ȼ����ֹ�����������gameObjectʱ����δ����״̬
        characterUI.SetActive(true);
        skillTreeUI.SetActive(true);
        craftUI.SetActive(true);
        optionsUI.SetActive(true);

        fadeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        SwitchTo(inGameUI);
    }

    void Update()
    {
        if (Input.GetButtonDown("UI_Character"))
        {
            SwitchWithKeyTo(characterUI);
        }

        if (Input.GetButtonDown("UI_SkillTree"))
        {
            SwitchWithKeyTo(skillTreeUI);
        }

        if (Input.GetButtonDown("UI_Craft"))
        {
            SwitchWithKeyTo(craftUI);
        }

        if (Input.GetButtonDown("UI_Options"))
        {
            SwitchWithKeyTo(optionsUI);
        }
    }

    public void SwitchTo(GameObject _memu)
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            bool isFadeScreen = (transform.GetChild(index).GetComponent<UI_FadeScreen>() != null);

            if (!isFadeScreen)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
        }

        if (_memu != null)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX("MenuConfirm", null);
            _memu.SetActive(true);
        }

        ServiceLocator.GetService<IGameManager>()?.PauseGame(_memu != inGameUI);
    }

    public void SwitchWithKeyTo(GameObject _memu)
    {
        if (_memu != null && _memu.activeSelf)
        {
            _memu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_memu);
    }

    private void CheckForInGameUI()
    {
        //ȷ��Ҫô���ĸ�ģ�鿪��һ����Ҫô����inGameUI
        for (int index = 0; index < transform.childCount; index++)
        {
            if (transform.GetChild(index).gameObject.activeSelf && transform.GetChild(index).GetComponent<UI_FadeScreen>() == null)
            {
                return;
            }
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    private IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(2.5f);

        endText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        restartButton.SetActive(true);

    }

    /// <summary>
    /// �����������ֵ����¿�ʼ�󣬴浵����ȡ�ļ������֮ǰû�д浵�Ļ�������ʾû������
    /// </summary>
    public void RestartGameButton() => ServiceLocator.GetService<IGameManager>().RestartScene();

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSetting)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parameter == pair.Key)
                {
                    item.LoadSlider(pair.Value);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSetting.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSetting.Add(item.parameter, item.slider.value);
        }
    }

    /// <summary>
    /// �˳���Ϸ
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();
    }
}
