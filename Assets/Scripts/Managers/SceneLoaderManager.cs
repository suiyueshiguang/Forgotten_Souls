using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour, ISceneLoaderManager
{
    [SerializeField] private string fadeScreenFullPath = "Canvas(Clone)/DarkScreen";

    private UI_FadeScreen fadeScreen;
    private AsyncOperation asyncLoad;

    private void Awake()
    {
        if (ServiceLocator.GetService<ISceneLoaderManager>() == null)
        {
            ServiceLocator.Register<ISceneLoaderManager>(this);
        }

        fadeScreen = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<UI_FadeScreen>(fadeScreenFullPath);
    }

    /// <summary>
    /// 预加载场景
    /// </summary>
    /// <param name="_sceneName">场景名称</param>
    public void PreLoadScene(string _sceneName)
    {
        /// 如果场景已被加载，则卸载场景
        if (SceneManager.GetSceneByName(_sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(_sceneName);

            StartCoroutine(UnloadAssets());
        }

        StartCoroutine(LoadSceneCoroutine(_sceneName, false));
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="_sceneName">场景名称</param>
    public void LoadScene(string _sceneName)
    {
        fadeScreen.FadeOut();

        StartCoroutine(LoadSceneCoroutine(_sceneName, true));

        fadeScreen.FadeIn();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="_sceneName">场景名称</param>
    /// <returns>返回协程</returns>
    private IEnumerator LoadSceneCoroutine(string _sceneName, bool _activationOnLoad)
    {
        Scene targetScene = SceneManager.GetSceneByName(_sceneName);

        // 场景已被加载时
        if (asyncLoad != null && asyncLoad.progress == 0.9f && _activationOnLoad)
        {
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // 设置新场景为活动场景
            SceneManager.SetActiveScene(targetScene);

            yield break;
        }

        //新场景加载流程
        if (!targetScene.isLoaded)
        {
            asyncLoad = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    private IEnumerator UnloadAssets()
    {
        AsyncOperation resourcesAsyncOperation = Resources.UnloadUnusedAssets();

        //进行卸载未使用的资源(Unity 自带)
        while (!resourcesAsyncOperation.isDone)
        {
            yield return null;
        }

        /*
         * 不建议手动使用下面方法，虽然能够有效卸载资源，但是会严重降低性能
         *
        for(int index = 0; index < 3; index++)
        {
            Debug.Log(System.GC.GetTotalMemory(false));

            //进行强制垃圾回收(微软 自带)
            System.GC.Collect();
            yield return null;
        }
        */
    }
}
