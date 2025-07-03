using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("技能信息")]
    public float cooldown;
    public float cooldownTimer;
    [SerializeField] protected string skillSoundName;

    protected IEventSystemManager eventSystemManager => eventSystemManagerValue ??= ServiceLocator.GetService<IEventSystemManager>();
    private IEventSystemManager eventSystemManagerValue;
    protected Player player => playerValue ??= ServiceLocator.GetService<IPlayerManager>().GetPlayer();
    private Player playerValue;

    protected virtual void Start()
    {
        checkUnlock();
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    protected virtual void checkUnlock()
    {
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        player.fx.CreatePopUpText("技能还在冷却");

        return false;
    }

    public virtual void UseSkill()
    {
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 18);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }
}
