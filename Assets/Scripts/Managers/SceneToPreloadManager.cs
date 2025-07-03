using UnityEngine;

public class SceneToPreloadManager : MonoBehaviour
{
    /// <summary>
    /// Ô¤¼ÓÔØsceneToPreloadµÄ³¡¾°
    /// </summary>
    private void Start()
    {
        foreach (Transform _sceneName in transform)
        {
            ServiceLocator.GetService<ISceneLoaderManager>().PreLoadScene(_sceneName.name);
        }
    }
}
