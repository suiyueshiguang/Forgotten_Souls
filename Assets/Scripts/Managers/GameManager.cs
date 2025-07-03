using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager, IGameManager
{
    private Transform playerTransform;

    [SerializeField] private Checkpoint[] checkpoints;
    private string closestCheckpointLoaded;

    [Header("丢失魂")]
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

        //获取所有检查点，不包括非活动的
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        playerTransform = ServiceLocator.GetService<IPlayerManager>().GetPlayer().transform;
    }

    /// <summary>
    /// 死亡后保存游戏切换界面
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
    /// 将玩家放置在离他检查点最近的地方
    /// </summary>
    private void LoadClosestCheckpoint()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointLoaded == checkpoint.id)
            {
                //存在小bug,玩家的中心和火把的中心不在同一个点上，导致存在下落趋势
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
    /// 激活存档文件中已经激活的检查点
    /// </summary>
    /// <param name="_data">存档文件</param>
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
    /// 找到离玩家最近的检查点并且是激活状态的
    /// </summary>
    /// <returns>返回离玩家最近的检查点</returns>
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
    /// 游戏暂停
    /// </summary>
    public void PauseGame(bool _pause) => Time.timeScale = _pause ? 0 : 1;
}
