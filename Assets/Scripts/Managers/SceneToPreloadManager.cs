using UnityEngine;

public class SceneToPreloadManager : MonoBehaviour
{
    /// <summary>
    /// Ԥ����sceneToPreload�ĳ���
    /// </summary>
    private void Start()
    {
        foreach (Transform _sceneName in transform)
        {
            ServiceLocator.GetService<ISceneLoaderManager>().PreLoadScene(_sceneName.name);
        }
    }
}
