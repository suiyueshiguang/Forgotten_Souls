using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager, IPlayerManager
{
    [SerializeField] private string playerFullPath;
    [field: SerializeField] public int currency { get; set; }

    private GameObject playerGameObject;
    private Player player;

    private void Awake()
    {
        if (ServiceLocator.GetService<IPlayerManager>() == null)
        {
            ServiceLocator.Register<IPlayerManager>(this);
        }

        playerGameObject = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<Transform>(playerFullPath).gameObject;
        player = playerGameObject.GetComponent<Player>();
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("没有足够的经验");
            return false;
        }

        currency -= _price;
        return true;
    }

    public GameObject GetPlayerGameObject() => playerGameObject;

    public Player GetPlayer() => player;

    public void LoadData(GameData _data) => currency = _data.currency;

    public void SaveData(ref GameData _data) => _data.currency = currency;
}
