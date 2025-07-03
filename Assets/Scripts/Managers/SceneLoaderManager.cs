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
    /// Ԥ���س���
    /// </summary>
    /// <param name="_sceneName">��������</param>
    public void PreLoadScene(string _sceneName)
    {
        /// ��������ѱ����أ���ж�س���
        if (SceneManager.GetSceneByName(_sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(_sceneName);

            StartCoroutine(UnloadAssets());
        }

        StartCoroutine(LoadSceneCoroutine(_sceneName, false));
    }

    /// <summary>
    /// ���س���
    /// </summary>
    /// <param name="_sceneName">��������</param>
    public void LoadScene(string _sceneName)
    {
        fadeScreen.FadeOut();

        StartCoroutine(LoadSceneCoroutine(_sceneName, true));

        fadeScreen.FadeIn();
    }

    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <param name="_sceneName">��������</param>
    /// <returns>����Э��</returns>
    private IEnumerator LoadSceneCoroutine(string _sceneName, bool _activationOnLoad)
    {
        Scene targetScene = SceneManager.GetSceneByName(_sceneName);

        // �����ѱ�����ʱ
        if (asyncLoad != null && asyncLoad.progress == 0.9f && _activationOnLoad)
        {
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // �����³���Ϊ�����
            SceneManager.SetActiveScene(targetScene);

            yield break;
        }

        //�³�����������
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
    /// ж����Դ
    /// </summary>
    private IEnumerator UnloadAssets()
    {
        AsyncOperation resourcesAsyncOperation = Resources.UnloadUnusedAssets();

        //����ж��δʹ�õ���Դ(Unity �Դ�)
        while (!resourcesAsyncOperation.isDone)
        {
            yield return null;
        }

        /*
         * �������ֶ�ʹ�����淽������Ȼ�ܹ���Чж����Դ�����ǻ����ؽ�������
         *
        for(int index = 0; index < 3; index++)
        {
            Debug.Log(System.GC.GetTotalMemory(false));

            //����ǿ����������(΢�� �Դ�)
            System.GC.Collect();
            yield return null;
        }
        */
    }
}
