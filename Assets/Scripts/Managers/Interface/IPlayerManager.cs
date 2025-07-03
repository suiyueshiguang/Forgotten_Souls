using UnityEngine;

public interface IPlayerManager
{
    public int currency { get; set; }

    public bool HaveEnoughMoney(int _price);
    public GameObject GetPlayerGameObject();
    public Player GetPlayer();
}
