using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [Header("�ƶ���Ϣ")]
    public float moveSpeed = 1.75f;
    public float battleMoveSpeedMultiply = 1.5f;
    public float idleTime = 2f;
    public float battleTime = 10;
    [SerializeField] protected string moveSoundName;
    private float defaultMoveSpeed;

    [Header("ѣ����Ϣ")]
    public float stunDuration = 1f;
    public Vector2 stunDirection = new Vector2(10f, 12f);
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    [Header("������Ϣ")]
    public float growDistance = 2.5f;
    public float attackDistance = 2f;
    public float AttackCooldown;
    public float minAttackCooldown = 0.15f;
    public float maxAttackCooldown = 0.3f;
    [HideInInspector] public float lastTimeAttacked;

    [SerializeField] protected LayerMask whatIsPlayer;

    public IEnemyStateMachine stateMachine { get; private set; }

    //�����߼�bug,entityӦ�ð������˺����
    public EntityFX fx { get; private set; }
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponent<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.GetCurrentState().Update();
    }

    public virtual void AssignLastAnimBool(string _animBoolName) => lastAnimBoolName = _animBoolName;

    public override void SlowEntityBy(float _slowPercentage, float _duration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        animator.speed = animator.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _duration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            animator.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            animator.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    //������(����Сbug,�������һֱ�������������߼��������߼��ϣ�����תΪidle״̬)
    public virtual RaycastHit2D IsPlayerDetected()
    {
        float playerDistanceCheck = 25;

        RaycastHit2D playerDetected = Physics2D.Raycast(new Vector2(wallCheck.position.x, wallCheck.position.y), Vector2.right * facingDir, playerDistanceCheck, whatIsPlayer);
        RaycastHit2D wallDetected = Physics2D.Raycast(new Vector2(wallCheck.position.x, wallCheck.position.y), Vector2.right * facingDir, playerDistanceCheck, whatIsGround);

        //ͬʱ��⵽ǽ�ں����
        if(playerDetected && wallDetected)
        {
            if(playerDetected.distance < wallDetected.distance)
            {
                return playerDetected;
            }

            return default;
        }

        return playerDetected;
    }

    public virtual void AnimationFinishTrigger() => stateMachine.GetCurrentState().AnimationFinishTrigger();

    public virtual void AnimationSpecialAttackTrigger()
    {
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

    public override void Die()
    {
        CloseCounterAttackWindow();
    }
}