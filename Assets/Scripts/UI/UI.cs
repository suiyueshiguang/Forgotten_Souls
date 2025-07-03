using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("结束背景")]
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
        //事前要先激活，防止后续激活或找gameObject时处于未激活状态
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
        //确保要么是四个模块开了一个，要么就是inGameUI
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
    /// 点击死亡后出现的重新开始后，存档并读取文件，如果之前没有存档的话，会显示没有数据
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
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
