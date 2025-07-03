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

    //ʹ��AES����
    private readonly string cipherSaveKey = "AES_Key";
    private readonly string ivSaveKey = "AES_IV";
    private byte[] cipher;
    private byte[] iv;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;
    private bool isQuitting;

    public static SaveManager instance;

    [ContextMenu("ɾ���浵")]
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

        //������뽨�黹�Ƿ���Awake����Ϊ��inventory���е�������Ҫ��start��ִ�У���load������
        saveManagers = FindAllSaveManager();

        //(Application.persistentDataPath)�־�������Ŀ¼��·�� ��ֻ����
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        //ע�⣬���ܴ���˳��bug�������LoadGame���ܻ������������ļ������ж�ȡ�ļ����������Ļ�û���жԳ����ĳ�ʼ��
        //����GameManager��checkpoints,�����checkpoints����start�У����´�������ִ������ģ�����chepoints == 0
        LoadCipherAndIV();
        LoadGame();
    }

    public void NewGame() => gameData = new GameData();

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("û����Ϸ����");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    /// <summary>
    /// ������Ϸ����
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
    /// ���ؼ�����Կ
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
    /// �����µļ�����Կ
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
    /// ����window������ƽ̨�ı���
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
    /// �����ƶ�ƽ̨������ƽ̨�ı���(����Ŀǰû�õ�)
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
    /// �ж����޴浵�ļ�
    /// </summary>
    /// <returns>���ؽ��,true��ʾ��,false��ʾ��</returns>
    public bool HasSaveData() => dataHandler.Load() != null;

}
