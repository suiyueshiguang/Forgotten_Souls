using UnityEngine;

public class UI_FadeScreen : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    /// <summary>
    /// ��͸�����ڲ���
    /// </summary>
    public void FadeOut() => animator.SetTrigger("fadeOut");

    /// <summary>
    /// �Ӻڲ�����͸��
    /// </summary>
    public void FadeIn() => animator.SetTrigger("fadeIn");
}
