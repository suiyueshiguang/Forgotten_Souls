using UnityEngine;

public interface IDialogueManager
{
    public System.Action<string, string> onDailyConversation { get; set; }

    /// <summary>
    /// �����ճ��Ի�Э��
    /// </summary>
    /// <param name="_triggerType">��������</param>
    /// <param name="_npcDescription">��ɫ�趨</param>
    public void StartDailyConversation(string _triggerType, string _npcDescription);

    /// <summary>
    /// ��ʾ�Ի����Լ���һ�仰
    /// </summary>
    /// <param name="_title">��ɫ��ν</param>
    /// <param name="_npcName">��ɫ����</param>
    /// <param name="_newLines">��ɫ�Ի�����</param>
    public void ShowDialogue(string _title, string _npcName, string[] _newLines);

    /// <summary>
    /// ��ʾ�ճ��Ի�
    /// </summary>
    /// <param name="_text">Ҫ������ı��ַ���</param>
    /// <param name="_textLifeTime">�ı�����ʱ��</param>
    /// <param name="_targetPosition">Ŀ��λ��</param>
    public void ShowDailyConversation(string _text, float _textLifeTime, Vector3 _targetPosition);

    /// <summary>
    /// �˳��Ի�
    /// </summary>
    public void QuitChatting();
}
