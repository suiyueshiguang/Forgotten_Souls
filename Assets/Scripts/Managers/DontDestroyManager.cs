using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyManager : MonoBehaviour, IDontDestroyManager
{
    [Header("取消注销")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerManager;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject audioManager;
    [SerializeField] private GameObject inventoryManager;
    [SerializeField] private GameObject saveManager;
    [SerializeField] private GameObject skillManager;
    [SerializeField] private GameObject light2D;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject sceneLoaderManager;
    [SerializeField] private GameObject dialogueManager;
    [SerializeField] private GameObject eventSystemManager;
    [SerializeField] private GameObject initiaDisObjectData;

    private static Dictionary<string, Transform> pathCache;

    private void Awake()
    {
        if (ServiceLocator.GetService<IDontDestroyManager>() == null)
        {
            pathCache = new Dictionary<string, Transform>();

            ServiceLocator.Register<IDontDestroyManager>(this);
            DontDestroyOnLoad(gameObject);
            InitializeGameObject();
        }
    }

    /// <summary>
    /// 取消注销GameObject
    /// </summary>
    private void InitializeGameObject()
    {
        //注意:以下代码的执行顺序涉及到谁先谁后的顺序执行,具体看unity官网的提及的生命周期
        Instantiate(audioManager, transform);
        Instantiate(inventoryManager, transform);
        Instantiate(player, transform);
        Instantiate(playerManager, transform);
        Instantiate(gameManager, transform);
        Instantiate(eventSystemManager, transform);
        Instantiate(canvas, transform);
        Instantiate(initiaDisObjectData, transform);
        Instantiate(dialogueManager, transform);
        Instantiate(skillManager, transform);
        Instantiate(light2D, transform);
        Instantiate(eventSystem, transform);
        Instantiate(sceneLoaderManager, transform);
        Instantiate(saveManager, transform);
    }

    /// <summary>
    /// 安全销毁DontDestroyOnLoad内GameObject的方法
    /// </summary>
    public void DestroyGlobalManagers()
    {
        if (ServiceLocator.GetService<IDontDestroyManager>() == null)
        {
            return;
        }

        // 销毁所有子对象
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 移除全部服务
        ServiceLocator.RemoveAllService();
    }

    /// <summary>
    /// 用于获取类型T,相对路径为_fullPath的unity游戏对象
    /// </summary>
    /// <typeparam name="T">需要获取的类型T</typeparam>
    /// <param name="_fullPath">需要查找的对象的相对路径</param>
    /// <returns>返回特定游戏对象的组件为T</returns>
    public T GetSceneData<T>(string _fullPath) where T : class
    {
        if (string.IsNullOrEmpty(_fullPath))
        {
            return null;
        }

        if (pathCache.TryGetValue(_fullPath, out Transform cachedTransform))
        {
            return cachedTransform.GetComponent<T>();
        }

        //分割路径
        string[] pathParts = _fullPath.Split('/');
        Transform target = FindInactiveObject(pathParts);

        if (target != null)
        {
            pathCache[_fullPath] = target;
            return target.GetComponent<T>();
        }

        Debug.Log($"未能找到路径:{_fullPath}");
        return null;
    }

    private Transform FindInactiveObject(string[] _pathParts)
    {
        //获取场景内以及在DontDestroyOnLoad的根对象（包括未激活的）
        GameObject[] rootObjectsInDontDestroy = gameObject.scene.GetRootGameObjects();
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject rootObject in rootObjectsInDontDestroy)
        {
            Transform result = RecursiveFind(rootObject.transform, _pathParts, 0);

            if (result != null)
            {
                return result;
            }
        }

        foreach (GameObject rootObject in rootObjects)
        {
            Transform result = RecursiveFind(rootObject.transform, _pathParts, 0);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private Transform RecursiveFind(Transform _current, string[] _pathParts, int _depth)
    {
        //查看当前层级名称是否匹配
        if (_current.name != _pathParts[_depth])
        {
            return null;
        }

        //到达路径末端
        if (_depth == _pathParts.Length - 1)
        {
            return _current;
        }

        //继续递归
        foreach (Transform _child in _current)
        {
            Transform result = RecursiveFind(_child, _pathParts, _depth + 1);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}