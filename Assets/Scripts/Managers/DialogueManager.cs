using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueManager : MonoBehaviour, IDialogueManager
{
    [SerializeField] private string urgentBoxFullPath;
    [SerializeField] private float textScrollSpeed;
    [SerializeField] private GameObject dialogueTextPrefab;

    [Header("API配置")]
    [SerializeField] private string apiEndpoint = "https://api.deepseek.com/v1/chat/completions";
    [SerializeField] private string apiKey;

    private RectTransform dialogueBox;
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI dialogueName;

    private string[] dialogueLines;
    private int dialogueCurrentLine = 0;
    private bool isScrolling;

    private Coroutine dailyConversation;

    public System.Action<string, string> onDailyConversation { get; set; }

    #region API数据结构
    [System.Serializable]
    private class RequestData
    {
        public Message[] messages;
        public string model;
        public float temperature;
        public int max_tokens;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class APIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }
    #endregion

    private void Start()
    {
        ServiceLocator.Register<IDialogueManager>(this);

        dialogueBox = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(urgentBoxFullPath);

        TextMeshProUGUI[] childTextMeshProUGUI = dialogueBox.GetComponentsInChildren<TextMeshProUGUI>(true);

        dialogueText = childTextMeshProUGUI[0];
        dialogueName = childTextMeshProUGUI[1];
    }

    private void Update()
    {
        if (dialogueBox.gameObject.activeInHierarchy && Input.GetButtonDown("LeftClick"))
        {
            if (isScrolling)
            {
                StopCoroutine("ScrollingText");
                dialogueText.text = dialogueLines[dialogueCurrentLine];
                isScrolling = false;

                return;
            }

            dialogueCurrentLine++;

            if (dialogueCurrentLine < dialogueLines.Length)
            {
                StartCoroutine("ScrollingText");
            }
            else
            {
                QuitChatting();
            }
        }
    }

    /// <summary>
    /// 退出对话
    /// </summary>
    public void QuitChatting()
    {
        dialogueCurrentLine = 0;
        dialogueBox.gameObject.SetActive(false);
        AbstractEventHandle.PopEventLevel(InputType.LeftClick);
    }

    /// <summary>
    /// 显示对话框以及第一句话
    /// </summary>
    /// <param name="_title">角色称谓</param>
    /// <param name="_npcName">角色名字</param>
    /// <param name="_newLines">角色对话内容</param>
    public void ShowDialogue(string _title, string _npcName, string[] _newLines)
    {
        if (!dialogueBox.gameObject.activeInHierarchy)
        {
            dialogueLines = _newLines;

            dialogueBox.gameObject.SetActive(true);

            dialogueName.text = $"{_title} <size=120>\u3000{_npcName} </size>";
            StartCoroutine("ScrollingText");
        }
    }

    /// <summary>
    /// 字体滚动效果
    /// </summary>
    private IEnumerator ScrollingText()
    {
        isScrolling = true;

        dialogueText.text = "";

        foreach (char letter in dialogueLines[dialogueCurrentLine].ToCharArray())
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(textScrollSpeed);
        }

        isScrolling = false;
    }

    /// <summary>
    /// 启动日常对话协程
    /// </summary>
    /// <param name="_triggerType">触发类型</param>
    /// <param name="_npcDescription">角色设定</param>
    public void StartDailyConversation(string _triggerType, string _npcDescription) => StartCoroutine(DailyConversation(_triggerType, _npcDescription));

    /// <summary>
    /// 利用DeepSeek生成日常对话
    /// </summary>
    /// <param name="_triggerType">触发类型</param>
    /// <param name="_npcDescription">角色设定</param>
    private IEnumerator DailyConversation(string _triggerType, string _npcDescription)
    {
        string prompt = $"{_npcDescription}\n根据以上设定生成{_triggerType}场景的对话，要求：\n- 长度不超过15字\n- 符合人物性格\n- 使用口语化表达\n- 只需要回答一句话即可，回答的主人公是本人，并非玩家";

        //与WEB服务器通话
        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            /*设置请求头设置成自定义
             * Content-Type: WEB 告诉自己响应的对象的类型。
             * Authorization: 当客户端接收到来自WEB服务器的 WWW-Authenticate 响应时，用该头部来回应自己的身份验证信息给WEB服务器
             * Accept: 告诉WEB服务器自己接受什么介质类型，/* 表示任何类型，type/* 表示该类型下的所有子类型
             */
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Accept", "application/json");

            RequestData requestData = new RequestData
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new Message
                    {
                        role = "system",
                        content = prompt
                    }
                },
                temperature = Mathf.Clamp(0.7f, 0f, 2f),
                max_tokens = Mathf.Clamp(30, 1, 4096)
            };

            // 将请求数据转为JSON字符串
            string jsonData = JsonUtility.ToJson(requestData);
            // JSON字符串编码为一组UTF-8字节数组
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            // 配置HTTP请求的上传处理
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //配置HTTP请求的下载处理
            request.downloadHandler = new DownloadHandlerBuffer();

            // 发送HTTP请求并等待完成
            yield return request.SendWebRequest();

            // 验证请求结果状态
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 将响应JSON文本反序列化为数据结构
                APIResponse response = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                // 提取生成的对话文本
                string generatedText = response.choices[0].message.content;
                // 缓存生成的文本
                onDailyConversation?.Invoke(_triggerType, generatedText);
            }
            else
            {
                //记录请求错误
                Debug.LogError($"API请求失败：{request.error}");
            }
        }
    }

    /// <summary>
    /// 显示日常对话
    /// </summary>
    /// <param name="_text">要处理的文本字符串</param>
    /// <param name="_textLifeTime">文本存在时间</param>
    /// <param name="_targetPosition">目标位置</param>
    public void ShowDailyConversation(string _text, float _textLifeTime, Vector3 _targetPosition)
    {
        GameObject newText = Instantiate(dialogueTextPrefab, GetRandomPosition(_targetPosition), Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = ProcessText(_text);
        newText.SetActive(true);
        Destroy(newText, _textLifeTime);
    }

    /// <summary>
    /// 定位在当前位置的正上方
    /// </summary>
    /// <returns>返回Vector3位置</returns>
    private Vector3 GetRandomPosition(Vector3 _targetPosition) => _targetPosition + new Vector3(0, Random.Range(2f, 3.5f), 0);

    /// <summary>
    /// 删除特定的尾随字符
    /// </summary>
    /// <param name="_input">字符串输入</param>
    /// <returns>返回处理后的字符串</returns>
    private string ProcessText(string _input) => _input.Trim().TrimEnd('。', '！', '？');
}
