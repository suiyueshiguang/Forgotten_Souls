using System.Collections;
using UnityEngine;

public enum SlimeType { big, medium, small }

public class Enemy_Slime : Enemy
{
    [Header("史莱姆类型")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int slimesToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;

    public SlimeStateFactory slimeStateFactory;

    private float fsxTimer;

    [Header("声效延迟时间")]
    [SerializeField] private float fsxMoveTime;
    [SerializeField] private float fsxBattleTime;

    #region state
    public string idleState { get; private set; } = "Idle";
    public string moveState { get; private set; } = "Move";
    public string battleState { get; private set; } = "Battle";
    public string attackState { get; private set; } = "Attack";
    public string stunnedState { get; private set; } = "Stunned";
    public string deadState { get; private set; } = "Die";
    #endregion

    protected override void Awake()
    {
        base.Awake();

        //由于史莱姆的动画默认朝左，需要把朝向改一下
        SetupDefailtFacingDir(-1);
    }

    protected override void Start()
    {
        base.Start();

        slimeStateFactory = new SlimeStateFactory(this, stateMachine);
        slimeStateFactory.InitializedSlimeState();
    }

    protected override void Update()
    {
        base.Update();

        /*
        fsxTimer -= Time.deltaTime;

        if (animator.speed != 0 && fsxTimer < 0)
        {
            SlimeMoveSFX();
        }
        */
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            slimeStateFactory.ChangeSlimeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        slimeStateFactory.ChangeSlimeState(deadState);

        if (slimeType == SlimeType.small)
        {
            return;
        }

        CreateSlime(slimesToCreate, slimePrefab);
    }

    private void CreateSlime(int _amountOfSlimes, GameObject _slimePrefab)
    {
        for (int index = 0; index < _amountOfSlimes; index++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir);
        }
    }

    public void SetupSlime(int _facingDir)
    {
        if (_facingDir != facingDir)
        {
            Filp();
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CreateKnockback(1.5f));
        }
    }

    private IEnumerator CreateKnockback(float _second)
    {
        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDir, yVelocity);

        yield return new WaitForSeconds(_second);

        isKnocked = false;
    }

    private void SlimeMoveSFX()
    {
        if (slimeStateFactory.slimeState is SlimeMoveState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxMoveTime;
        }

        if (slimeStateFactory.slimeState is SlimeBattleState)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(moveSoundName, transform);
            fsxTimer = fsxBattleTime;
        }
    }
}
