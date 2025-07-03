using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    [Header("克隆信息")]
    [SerializeField] private float colorLoosingSpeed;
    [SerializeField] private Transform attackChecks;
    [SerializeField] private float attackCheckRadius = .8f;
    private float cloneTimer;
    private Transform closestEnemy;
    private SpriteRenderer sr;
    private Animator animator;
    private bool canDuplicateClone;
    private float chanceToDuplicate;
    private int facingDir = 1;
    private Player player;
    private float attackMutiplier;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        
        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if(sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetUpClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicateClone, float _chanceToDuplicate, float _attackMultiplier)
    {
        //定义分身攻击的种类
        if (_canAttack)
        {
            animator.SetInteger("AttackNumber", Random.Range(1, 4));
        }

        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        attackMutiplier = _attackMultiplier;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = .1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackChecks.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnowbackDir(transform);

                Player player = ServiceLocator.GetService<IPlayerManager>().GetPlayer();

                player.GetComponent<PlayerStats>().CloneDoDamage(hit.GetComponent<EnemyStats>(), attackMutiplier);

                if(player.skill.GetClone().canApplyOnHitEffect)
                {
                    ServiceLocator.GetService<IInventory>()?.GetEquipment(EquipmentType.Weapon)?.Effect(hit.transform);
                }

                //由幻影产生幻影
                if (canDuplicateClone)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(hit.transform,  new Vector3(1.25f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
