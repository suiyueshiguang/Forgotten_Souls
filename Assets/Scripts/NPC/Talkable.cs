using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [Header("�Ի�����")]
    [SerializeField] private string title;
    [SerializeField] private string npcName;
    [TextArea(1, 3)]
    [SerializeField] private string[] lines;
    [SerializeField] private bool aiDailyConversation;

    [Header("NPC�趨")]
    [TextArea(3, 10)]
    [SerializeField] private string npcDescription;

    [Header("�Ի��趨")]
    [SerializeField] private string defaultEnterScene;
    [SerializeField] private string defaultExitScene;
    //[SerializeField] private float textLifeTime = 2.5f;
    [SerializeField] private float cooldownTime = 2.5f;

    private Dictionary<string, string> dialogueCache = new Dictionary<string, string>();
    private IDialogueManager dialogueManager;
    private AbstractEventHandle eventHandle;

    private float lastRequestTime = 0;
    private bool isEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ShowDailyDialogue("enter");

            isEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ShowDailyDialogue("exit");

            isEntered = false;

            dialogueManager?.QuitChatting();
        }
    }

    private void Start()
    {
        dialogueManager = ServiceLocator.GetService<IDialogueManager>();

        dialogueManager.onDailyConversation += UpdateDictionary;

        if (aiDailyConversation)
        {
            dialogueManager.StartDailyConversation("enter", npcDescription);
            dialogueManager.StartDailyConversation("exit", npcDescription);
        }

        lastRequestTime = Time.time;
    }

    private void Update()
    {
        //��ʼ���о���Ի�
        if (isEntered && Input.GetButtonDown("Interactive"))
        {
            eventHandle = new DialogueHandler(InputType.LeftClick, EventLevelType.middle);
            eventHandle.EventHandle(() => { dialogueManager.ShowDialogue(title, npcName, lines); });
        }
    }

    private void OnDestroy()
    {
        if (dialogueManager == null)
        {
            return;
        }

        dialogueManager.onDailyConversation -= UpdateDictionary;
    }

    /// <summary>
    /// ���»���
    /// </summary>
    /// <param name="_triggerType">��������</param>
    /// <param name="_generatedText">��ɫ�Ի�����</param>
    private void UpdateDictionary(string _triggerType, string _generatedText)
    {
        if (dialogueCache.ContainsKey(_triggerType))
        {
            dialogueCache[_triggerType] = _generatedText;
        }
        else
        {
            dialogueCache.Add(_triggerType, _generatedText);
        }
    }

    /// <summary>
    /// ��ʾÿ�նԻ�
    /// </summary>
    /// <param name="_triggerType">��������</param>
    private void ShowDailyDialogue(string _triggerType)
    {
        if (!aiDailyConversation)
        {
            dialogueManager.ShowDailyConversation(_triggerType.Equals("enter") ? defaultEnterScene : defaultExitScene, cooldownTime, transform.position);
            return;
        }

        if (Time.time > lastRequestTime + cooldownTime)
        {
            // ��黺�����Ƿ���ڸ����͵ĶԻ����������ֱ����ʾ���������API���󲢻�����
            if (dialogueCache.TryGetValue(_triggerType, out string cachedText))
            {
                dialogueManager.ShowDailyConversation(cachedText, cooldownTime, transform.position);
            }
            else
            {
                dialogueManager.ShowDailyConversation(_triggerType.Equals("enter") ? defaultEnterScene : defaultExitScene, cooldownTime, transform.position);
                dialogueManager.StartDailyConversation(_triggerType, npcDescription);
            }

            lastRequestTime = Time.time;
        }
    }
}
