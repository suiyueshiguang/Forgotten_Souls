using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScreen fadeScreen;

    private AsyncOperation asyncLoad;

    private void Start()
    {
        continueButton.SetActive(SaveManager.instance.HasSaveData());
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect());
    }

    /// <summary>
    /// 开始新的游戏
    /// </summary>
    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect());
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }

    private IEnumerator LoadSceneWithFadeEffect()
    {
        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        fadeScreen.FadeOut();

        asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        fadeScreen.FadeIn();

        SceneManager.SetActiveScene(targetScene);
    }
}
