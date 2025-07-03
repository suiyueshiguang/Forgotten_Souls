using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;

    [Header("检查点信息")]
    public string id;
    public bool activationStatus;
    [SerializeField] private string checkpointSoundName;

    private Light2D light2D;

    /// <summary>
    /// 在最开始加载时设置灯光状态未false，如果放在Start里会导致是先加载存档后再加载Start,灯光错误显示
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

    [ContextMenu("生成检查点ID")]
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
