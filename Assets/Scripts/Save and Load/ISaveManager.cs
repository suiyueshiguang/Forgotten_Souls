public interface ISaveManager
{
    void LoadData(GameData _data);

    //ref�ؼ���(������ȡ����Ϣ���и���,��Ӱ��ԭ������)
    void SaveData(ref GameData _data);
}
