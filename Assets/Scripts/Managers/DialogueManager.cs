using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueManager : MonoBehaviour, IDialogueManager
{
    [SerializeField] private string urgentBoxFullPath;
    [SerializeField] private float textScrollSpeed;
    [SerializeField] private GameObject dialogueTextPrefab;

    [Header("API����")]
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

    #region API���ݽṹ
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
    /// �˳��Ի�
    /// </summary>
    public void QuitChatting()
    {
        dialogueCurrentLine = 0;
        dialogueBox.gameObject.SetActive(false);
        AbstractEventHandle.PopEventLevel(InputType.LeftClick);
    }

    /// <summary>
    /// ��ʾ�Ի����Լ���һ�仰
    /// </summary>
    /// <param name="_title">��ɫ��ν</param>
    /// <param name="_npcName">��ɫ����</param>
    /// <param name="_newLines">��ɫ�Ի�����</param>
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
    /// �������Ч��
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
    /// �����ճ��Ի�Э��
    /// </summary>
    /// <param name="_triggerType">��������</param>
    /// <param name="_npcDescription">��ɫ�趨</param>
    public void StartDailyConversation(string _triggerType, string _npcDescription) => StartCoroutine(DailyConversation(_triggerType, _npcDescription));

    /// <summary>
    /// ����DeepSeek�����ճ��Ի�
    /// </summary>
    /// <param name="_triggerType">��������</param>
    /// <param name="_npcDescription">��ɫ�趨</param>
    private IEnumerator DailyConversation(string _triggerType, string _npcDescription)
    {
        string prompt = $"{_npcDescription}\n���������趨����{_triggerType}�����ĶԻ���Ҫ��\n- ���Ȳ�����15��\n- ���������Ը�\n- ʹ�ÿ��ﻯ���\n- ֻ��Ҫ�ش�һ�仰���ɣ��ش�����˹��Ǳ��ˣ��������";

        //��WEB������ͨ��
        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            /*��������ͷ���ó��Զ���
             * Content-Type: WEB �����Լ���Ӧ�Ķ�������͡�
             * Authorization: ���ͻ��˽��յ�����WEB�������� WWW-Authenticate ��Ӧʱ���ø�ͷ������Ӧ�Լ��������֤��Ϣ��WEB������
             * Accept: ����WEB�������Լ�����ʲô�������ͣ�/* ��ʾ�κ����ͣ�type/* ��ʾ�������µ�����������
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

            // ����������תΪJSON�ַ���
            string jsonData = JsonUtility.ToJson(requestData);
            // JSON�ַ�������Ϊһ��UTF-8�ֽ�����
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            // ����HTTP������ϴ�����
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //����HTTP��������ش���
            request.downloadHandler = new DownloadHandlerBuffer();

            // ����HTTP���󲢵ȴ����
            yield return request.SendWebRequest();

            // ��֤������״̬
            if (request.result == UnityWebRequest.Result.Success)
            {
                // ����ӦJSON�ı������л�Ϊ���ݽṹ
                APIResponse response = JsonUtility.FromJson<APIResponse>(request.downloadHandler.text);
                // ��ȡ���ɵĶԻ��ı�
                string generatedText = response.choices[0].message.content;
                // �������ɵ��ı�
                onDailyConversation?.Invoke(_triggerType, generatedText);
            }
            else
            {
                //��¼�������
                Debug.LogError($"API����ʧ�ܣ�{request.error}");
            }
        }
    }

    /// <summary>
    /// ��ʾ�ճ��Ի�
    /// </summary>
    /// <param name="_text">Ҫ������ı��ַ���</param>
    /// <param name="_textLifeTime">�ı�����ʱ��</param>
    /// <param name="_targetPosition">Ŀ��λ��</param>
    public void ShowDailyConversation(string _text, float _textLifeTime, Vector3 _targetPosition)
    {
        GameObject newText = Instantiate(dialogueTextPrefab, GetRandomPosition(_targetPosition), Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = ProcessText(_text);
        newText.SetActive(true);
        Destroy(newText, _textLifeTime);
    }

    /// <summary>
    /// ��λ�ڵ�ǰλ�õ����Ϸ�
    /// </summary>
    /// <returns>����Vector3λ��</returns>
    private Vector3 GetRandomPosition(Vector3 _targetPosition) => _targetPosition + new Vector3(0, Random.Range(2f, 3.5f), 0);

    /// <summary>
    /// ɾ���ض���β���ַ�
    /// </summary>
    /// <param name="_input">�ַ�������</param>
    /// <returns>���ش������ַ���</returns>
    private string ProcessText(string _input) => _input.Trim().TrimEnd('��', '��', '��');
}
