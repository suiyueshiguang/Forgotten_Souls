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
    /// 从透明到黑布景
    /// </summary>
    public void FadeOut() => animator.SetTrigger("fadeOut");

    /// <summary>
    /// 从黑布景到透明
    /// </summary>
    public void FadeIn() => animator.SetTrigger("fadeIn");
}
