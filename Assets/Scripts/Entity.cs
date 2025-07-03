using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("碰撞信息")]
    public Transform attackCheck;
    public float attackCheckRadius = 1.99f;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 1f;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = 1.5f;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("击退信息")]
    [SerializeField] protected Vector2 knockbackPower = new Vector2(7f, 12f);
    [SerializeField] protected Vector2 knockbackOffset = new Vector2(.5f, 2f);
    [SerializeField] protected float knockbackDuration = 0.07f;
    protected bool isKnocked;

    #region Components
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    public int knockbackDir { get; private set; }

    public int facingDir { get; private set; } = 1;
    public bool facingRight = true;

    public System.Action onFilpped;

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _duration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        animator.speed = 1;
    }

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    public virtual void SetupKnowbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
        {
            knockbackDir = -1;
        }
        else if (_damageDirection.position.x < transform.position.x)
        {
            knockbackDir = 1;
        }
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    public void SetupKnockbackPower(Vector2 _knowbackPower) => knockbackPower = _knowbackPower;

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(0, knockbackOffset.x);
        float yOffset = Random.Range(0, knockbackOffset.y);

        //由于当实体受到攻击后，速度降低到0，会出现移动时顿挫感，如果要取消这个效果，把取消下面的if语句的注释
        //if(knockbackPower.x > 0 ||knockbackPower.y > 0)
        rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y + yOffset);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;

        SetupZeroKnockbackPower();
    }

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
        {
            return;
        }

        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
        {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FilpController(_xVelocity);
    }
    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Filp
    public virtual void Filp()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFilpped != null)
        {
            onFilpped();
        }
    }

    //此有bug,当玩家一直在敌人头上跳时，敌人鬼畜翻转
    public virtual void FilpController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Filp();
        }
        else if (_x < 0 && facingRight)
        {
            Filp();
        }
    }

    public virtual void SetupDefailtFacingDir(int _direction)
    {
        facingDir = _direction;

        if(facingDir == -1)
        {
            facingRight = false;
        }
    }
    #endregion

    public virtual void Die()
    {
        //存在bug,当玩家被destroy后，点击重新开始按钮后，gameManager尝试调用离玩家最近的，但玩家已被消除了，无法查找
        //不过我认为可以通过checkpoint的触发器来记录最后经过的，而不是离玩家最近的
        StartCoroutine(DieSetBool());
    }

    private IEnumerator DieSetBool()
    {
        yield return null;

        animator.SetBool("Die", true);
    }
}
