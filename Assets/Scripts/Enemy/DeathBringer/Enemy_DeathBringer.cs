using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public DeathBringerStateFactory deathBringerStateFactory;
    public bool bossFightBegun;

    private float fsxTimer;


    [Header("传送信息")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    private float defaultChanceToTeleport = 30f;
    private float maxAttempts = 10f;


    [Header("施法信息")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private float distanceFromPlayerMul;
    [SerializeField] private float spellHeight;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;

    [Header("声效延迟时间")]
    [SerializeField] private float fsxMoveTime;
    [SerializeField] private float fsxBattleTime;

    #region state
    public string idleState { get; private set; } = "Idle";
    public string teleportState { get; private set; } = "Teleport";
    public string battleState { get; private set; } = "Battle";
    public string attackState { get; private set; } = "Attack";
    public string spellCastState { get; private set; } = "SpellCast";
    public string stunnedState { get; private set; } = "Stunned";
    public string deadState { get; private set; } = "Die";
    #endregion

    protected override void Awake()
    {
        base.Awake();

        //由于该敌人的动画默认朝左，需要把朝向改一下
        SetupDefailtFacingDir(-1);
    }

    protected override void Start()
    {
        base.Start();

        chanceToTeleport = defaultChanceToTeleport;

        deathBringerStateFactory = new DeathBringerStateFactory(this, stateMachine);

        deathBringerStateFactory.InitializedDeathBringerState();
    }

    protected override void Update()
    {
        base.Update();

        /*
        fsxTimer -= Time.deltaTime;

        if (animator.speed != 0 && fsxTimer < 0)
        {
            DeathBrinerMoveSFX();
        }
        */
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            deathBringerStateFactory.ChangeDeathBringerState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        deathBringerStateFactory.ChangeDeathBringerState(deadState);
    }

    public void CanSpell()
    {
        Player player = ServiceLocator.GetService<IPlayerManager>().GetPlayer();

        float xOffset = 0;
        
        if(player.rb.velocity.x != 0)
        {
            xOffset = player.facingDir * distanceFromPlayerMul;
        }

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellHeight);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpell_Controller>().SetupSpell(stats);
    }

    private void DeathBrinerMoveSFX()
    {
        if (deathBringerStateFactory.deathBringerState is DeathBringerBattleState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxBattleTime;
        }
    }

    public void FindPosition()
    {
        Vector3 defaultPosition = transform.position;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float x = Random.Range(arena.bounds.min.x + 2, arena.bounds.max.x - 2);
            float y = Random.Range(arena.bounds.min.y + 2, arena.bounds.max.y - 2);

            transform.position = new Vector3(x, y);
            //下面后面加的数字是偏移量，存在胶囊并不是完全的在transform.position，有必要用一个偏移量进行处理（bug）
            transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

            if (!GroundBelow() || SomethingIsAround())
            {
                Debug.Log("重新查找新点位");
                continue;
            }

            // 如果找到了合适的位置，直接返回
            return;
        }

        transform.position = defaultPosition;  
        Debug.Log("未能在最大尝试次数内找到合适的位置");
    }

    /// <summary>
    /// 检查下面距离100的有没有地面
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

    /// <summary>
    /// 防止出现敌人传送到墙壁里
    /// </summary>
    /// <returns></returns>
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //绘制一根线
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        //绘制线框
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if(Random.Range(0,100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellStateCooldown)
        { 
            return true;
        }

        return false;
    }
}
