using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    //使用AES加密
    private readonly string cipherSaveKey = "AES_Key";
    private readonly string ivSaveKey = "AES_IV";
    private byte[] cipher;
    private byte[] iv;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;
    private bool isQuitting;

    public static SaveManager instance;

    [ContextMenu("删除存档")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }

        //下面代码建议还是放在Awake，因为在inventory中有的数据需要在start中执行，和load关联到
        saveManagers = FindAllSaveManager();

        //(Application.persistentDataPath)持久性数据目录的路径 （只读）
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        //注意，可能存在顺序bug，下面的LoadGame可能会先于其他的文件来进行读取文件，而其他的还没进行对场景的初始化
        //例如GameManager的checkpoints,如果将checkpoints放入start中，导致代码是先执行下面的，导致chepoints == 0
        LoadCipherAndIV();
        LoadGame();
    }

    public void NewGame() => gameData = new GameData();

    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("没有游戏数据");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    /// <summary>
    /// 保存游戏数据
    /// </summary>
    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    /// <summary>
    /// 加载加密密钥
    /// </summary>
    private void LoadCipherAndIV()
    {
        if (PlayerPrefs.HasKey(cipherSaveKey) && PlayerPrefs.HasKey(ivSaveKey))
        {
            cipher = Convert.FromBase64String(PlayerPrefs.GetString(cipherSaveKey));
            iv = Convert.FromBase64String(PlayerPrefs.GetString(ivSaveKey));
        }

        dataHandler.UpdateCipherIV(cipher, iv);
    }

    /// <summary>
    /// 创建新的加密密钥
    /// </summary>
    private void CreateCipherAndIV()
    {
        using (Aes aes = Aes.Create())
        {
            cipher = aes.Key;
            iv = aes.IV;
        }

        dataHandler.UpdateCipherIV(cipher, iv);

        PlayerPrefs.SetString(cipherSaveKey, Convert.ToBase64String(cipher));
        PlayerPrefs.SetString(ivSaveKey, Convert.ToBase64String(iv));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 处理window等类似平台的保存
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        isQuitting = true;

        CreateCipherAndIV();
        SaveGame();
    }

    /// <summary>
    /// 处理移动平台等类似平台的保存(不过目前没用到)
    /// </summary>
    /// <param name="pauseStatus"></param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !isQuitting)
        {
            //SaveGame();
        }
    }

    private List<ISaveManager> FindAllSaveManager()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsByType(typeof(MonoBehaviour), FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    /// <summary>
    /// 判断有无存档文件
    /// </summary>
    /// <returns>返回结果,true表示有,false表示无</returns>
    public bool HasSaveData() => dataHandler.Load() != null;

}
