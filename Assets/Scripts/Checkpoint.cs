using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;

    [Header("������Ϣ")]
    public string id;
    public bool activationStatus;
    [SerializeField] private string checkpointSoundName;

    private Light2D light2D;

    /// <summary>
    /// ���ʼ����ʱ���õƹ�״̬δfalse���������Start��ᵼ�����ȼ��ش浵���ټ���Start,�ƹ������ʾ
    /// </summary>
    private void Awake()
    {

    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        light2D = GetComponentInChildren<Light2D>();

        light2D.enabled = false;
    }

    [ContextMenu("���ɼ���ID")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {   
        if (!activationStatus)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(checkpointSoundName, transform);
        }

        activationStatus = true;
        StartCoroutine(ActiveSetBool());
    }

    private IEnumerator ActiveSetBool()
    {
        yield return null;
        animator.SetBool("active", activationStatus);
        light2D.enabled = true;
    }
}
