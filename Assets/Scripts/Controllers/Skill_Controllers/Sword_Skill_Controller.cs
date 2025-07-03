using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isRetruning;

    private float returnSpeed = 12f;
    private float freezeTimeDuration;

    [Header("弹射剑")]
    public float bounceSpeed;
    public float swordBounceRadius;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("飞剑")]
    public int pierceAmount = 0;

    [Header("回旋剑")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpining;
    private float spinDirection;

    private float hitTimer;
    private float hitCooldown;
    private bool isFirstSpinDuration = true;

    //isEntering和后面的表示当剑插入到刚体后才能返回
    //private bool isEntering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        if (Vector2.Distance(gameObject.transform.position, player.transform.position) > 20)
        {
            Destroy(gameObject);
        }
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
        {
            anim.SetBool("Rotation", true);
        }

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetUpSpin(bool _isSpining, float _maxTraveDistance, float _spinDuration, float _hitCooldown)
    {
        isSpining = _isSpining;
        maxTravelDistance = _maxTraveDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    private void Update()
    {
        if (canRotate)
        {
            transform.right = rb.velocity;
        }

        //if(isRetruning && isEntering)
        if (isRetruning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1f)
            {
                player.CatchTheSword();
            }
        }

        BounceLogic();

        SpinLogic();
    }

    private void BounceLogic()
    {
        if (isBouncing)
        {
            enemyTarget.RemoveAll(target => target == null);

            if (enemyTarget.Count > 0 && enemyTarget.Count > targetIndex)
            {
                transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
                {
                    SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                    targetIndex++;
                    bounceAmount--;

                    if (bounceAmount <= 0)
                    {
                        isBouncing = false;
                        isRetruning = true;
                    }

                    if (targetIndex >= enemyTarget.Count)
                    {
                        targetIndex = 0;
                    }
                }
            }
        }
    }

    private void SpinLogic()
    {
        if (isSpining)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                //当停止后，可以像有惯性一样继续往前移动（如果不想要的话可以注释掉）
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isRetruning = true;
                    isSpining = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;

        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (isFirstSpinDuration)
        {
            spinTimer = spinDuration;
            isFirstSpinDuration = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //背后攻击敌人成为可能
        if (isRetruning)
        {
            return;
        }

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            //FilpWhenThrow(collision);

            SwordSkillDamage(enemy);
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        player.stats.DoDamage(enemyStats);

        if (player.skill.GetSword().timeStopUnlocked)
        {
            enemy.FreezeTimeFor(freezeTimeDuration);
        }

        if (player.skill.GetSword().vulnerableUnlocked)
        {
            Debug.Log("敌人挂上了脆弱属性");
            enemyStats.MakeVulnerableFor(freezeTimeDuration);
        }

        ItemData_Equipment equipeAmulet = ServiceLocator.GetService<IInventory>().GetEquipment(EquipmentType.Amulet);

        if (equipeAmulet != null)
        {
            equipeAmulet.Effect(enemy.transform);
        }
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, swordBounceRadius);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void FilpWhenThrow(Collider2D collision)
    {
        //投剑时转身
        if (((transform.position.x < collision.GetComponent<Enemy>().transform.position.x && collision.GetComponent<Enemy>().facingRight)
            || (transform.position.x > collision.GetComponent<Enemy>().transform.position.x && !collision.GetComponent<Enemy>().facingRight))
            && !isBouncing)
        {
            collision.GetComponent<Enemy>().Filp();
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpining)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;
        }

        anim.SetBool("Rotation", false);
        //isEntering = true;
        transform.parent = collision.transform;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform.parent = null;
        isRetruning = true;

        //技能剑的冷却时间
    }

}
