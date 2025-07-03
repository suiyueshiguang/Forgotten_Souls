using UnityEngine;

public interface IDialogueManager
{
    public System.Action<string, string> onDailyConversation { get; set; }

    /// <summary>
    /// 启动日常对话协程
    /// </summary>
    /// <param name="_triggerType">触发类型</param>
    /// <param name="_npcDescription">角色设定</param>
    public void StartDailyConversation(string _triggerType, string _npcDescription);

    /// <summary>
    /// 显示对话框以及第一句话
    /// </summary>
    /// <param name="_title">角色称谓</param>
    /// <param name="_npcName">角色名字</param>
    /// <param name="_newLines">角色对话内容</param>
    public void ShowDialogue(string _title, string _npcName, string[] _newLines);

    /// <summary>
    /// 显示日常对话
    /// </summary>
    /// <param name="_text">要处理的文本字符串</param>
    /// <param name="_textLifeTime">文本存在时间</param>
    /// <param name="_targetPosition">目标位置</param>
    public void ShowDailyConversation(string _text, float _textLifeTime, Vector3 _targetPosition);

    /// <summary>
    /// 退出对话
    /// </summary>
    public void QuitChatting();
}
