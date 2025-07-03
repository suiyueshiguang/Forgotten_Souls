using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager, IGameManager
{
    private Transform playerTransform;

    [SerializeField] private Checkpoint[] checkpoints;
    private string closestCheckpointLoaded;

    [Header("��ʧ��")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    public int lostCurrencyAmount { get; set; }

    private void Awake()
    {
        if (ServiceLocator.GetService<IGameManager>() == null)
        {
            ServiceLocator.Register<IGameManager>(GetComponent<IGameManager>());
        }

        //��ȡ���м��㣬�������ǻ��
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        playerTransform = ServiceLocator.GetService<IPlayerManager>().GetPlayer().transform;
    }

    /// <summary>
    /// �����󱣴���Ϸ�л�����
    /// </summary>
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();

        ServiceLocator.GetService<IDontDestroyManager>().DestroyGlobalManagers();

        SceneManager.LoadScene("MainScene");
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyX = playerTransform.position.x;
        _data.lostCurrencyY = playerTransform.position.y;
        _data.lostCurrencyAmount = lostCurrencyAmount;

        if (FindClosestCheckpoint() != null)
        {
            _data.closestCheckpointId = FindClosestCheckpoint().id;
        }

        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint == null)
            {
                continue;
            }

            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    public void LoadData(GameData _data)
    {
        closestCheckpointLoaded = _data.closestCheckpointId;
        LoadClosestCheckpoint();
        LoadCheckpoints(_data);
        LoadLostCurrency(_data);
    }

    /// <summary>
    /// ����ҷ�����������������ĵط�
    /// </summary>
    private void LoadClosestCheckpoint()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointLoaded == checkpoint.id)
            {
                //����Сbug,��ҵ����ĺͻ�ѵ����Ĳ���ͬһ�����ϣ����´�����������
                playerTransform.position = checkpoint.transform.position;
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    /// <summary>
    /// ����浵�ļ����Ѿ�����ļ���
    /// </summary>
    /// <param name="_data">�浵�ļ�</param>
    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }
    }

    /// <summary>
    /// �ҵ����������ļ��㲢���Ǽ���״̬��
    /// </summary>
    /// <returns>�������������ļ���</returns>
    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;

        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint == null)
            {
                continue;
            }

            float distanceToCheckpoint = Vector2.Distance(playerTransform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }

    /// <summary>
    /// ��Ϸ��ͣ
    /// </summary>
    public void PauseGame(bool _pause) => Time.timeScale = _pause ? 0 : 1;
}
