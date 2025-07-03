using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public DeathBringerStateFactory deathBringerStateFactory;
    public bool bossFightBegun;

    private float fsxTimer;


    [Header("������Ϣ")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    private float defaultChanceToTeleport = 30f;
    private float maxAttempts = 10f;


    [Header("ʩ����Ϣ")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private float distanceFromPlayerMul;
    [SerializeField] private float spellHeight;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;

    [Header("��Ч�ӳ�ʱ��")]
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

        //���ڸõ��˵Ķ���Ĭ�ϳ�����Ҫ�ѳ����һ��
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
            //�������ӵ�������ƫ���������ڽ��Ҳ�������ȫ����transform.position���б�Ҫ��һ��ƫ�������д���bug��
            transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

            if (!GroundBelow() || SomethingIsAround())
            {
                Debug.Log("���²����µ�λ");
                continue;
            }

            // ����ҵ��˺��ʵ�λ�ã�ֱ�ӷ���
            return;
        }

        transform.position = defaultPosition;  
        Debug.Log("δ��������Դ������ҵ����ʵ�λ��");
    }

    /// <summary>
    /// ����������100����û�е���
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);

    /// <summary>
    /// ��ֹ���ֵ��˴��͵�ǽ����
    /// </summary>
    /// <returns></returns>
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //����һ����
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        //�����߿�
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
