using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int currency;
    public float health;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, bool> checkpoints;
    public SerializableDictionary<string, int> inventory;

    public List<string> equipmentId;
    public string closestCheckpointId;

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSetting;

    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        currency = 0;
        closestCheckpointId = string.Empty;
        skillTree = new SerializableDictionary<string, bool>();
        checkpoints = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        volumeSetting = new SerializableDictionary<string, float>();
    }
}
