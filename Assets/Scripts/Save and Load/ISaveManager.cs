public interface ISaveManager
{
    void LoadData(GameData _data);

    //ref关键字(允许将获取的信息进行更改,会影响原本数据)
    void SaveData(ref GameData _data);
}
