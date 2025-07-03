using System;
using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critateChance,
    critatePower,
    health,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightningDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("主要数值")]
    public Stat strength;     // 力量(加一点物理攻击力和百分之一的力量)
    public Stat agility;      // 敏捷(加百分之一的闪避和百分之一的暴击率)
    public Stat intelligence; // 智力(加一点魔法攻击力和百分之三抵抗力)
    public Stat vitality;     // 体力(加五点血量)

    [Header("进攻数值")]
    public Stat damage;        // 伤害
    public Stat critateChance; // 暴击率
    public Stat critatePower;  // 暴击倍率默认为150%

    [Header("防御数值")]
    public Stat maxHealth;    //最大生命
    public Stat armor;        //防御(1000防御为满了)
    public Stat evasion;      //闪避
    public Stat magicResistance;

    [Header("魔法信息")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;    // 持续伤害
    public bool isChilled;    // 减少百分之20防御
    public bool isShocked;    // 减少百分之20的精准度

    [SerializeField] private float ailmentsDuration = 4f;
    private float ignitedTimer;
    private float chillTimer;
    private float shockTimer;

    private float igniteDamageCooldown = .3f;
    private float ignitedDamageTimer;
    private float igniteDamage;
    private float shockDamage;
    [SerializeField] private GameObject shockStrikePrefab;

    public float currentHealth;

    public System.Action onHealthChanged;
    public bool isInvincible { get; private set; }
    public bool isDead { get; private set; }
    private bool isVulnerable;

    protected virtual void Start()
    {
        critatePower.SetDefalutValut(150);
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockTimer -= Time.deltaTime;
        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }

        if (chillTimer < 0)
        {
            isChilled = false;
        }

        if (shockTimer < 0)
        {
            isShocked = false;
        }

        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }

    /// <summary>
    /// 启动VulnerableCoroutine(给目标挂上脆弱（减少10%防御）)协程
    /// </summary>
    /// <param name="_duration">debuff持续时间</param>
    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCoroutine(_duration));

    /// <summary>
    /// 给目标挂上脆弱（减少10%防御）
    /// </summary>
    /// <param name="_duration">debuff持续时间</param>
    /// <returns></returns>
    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(float _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(float _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
        {
            return;
        }

        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnowbackDir(transform);

        float totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCritialDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);

        //DoMagicDamage(_targetStats);
    }

    #region Magicl damage and ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        float _fireDamage = fireDamage.GetValue();
        float _iceDamage = iceDamage.GetValue();
        float _lightningDamage = lightningDamage.GetValue();

        float totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptyToApplyAlienance(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptyToApplyAlienance(CharacterStats _targetStats, float _fireDamage, float _iceDamage, float _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            float randomValue = UnityEngine.Random.value;

            if (randomValue < 0.33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            else if (randomValue < 0.66f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            else if (randomValue < 0.99f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(_fireDamage * 0.2f);
        }

        if (canApplyShock)
        {
            _targetStats.SetupShockStrikeDamage(_lightningDamage * 0.3f);
        }

        _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilment(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            if (!isDead)
            {
                fx.IgniteFXFor(ignitedTimer);
            }
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chillTimer = ailmentsDuration;

            float slowPercentage = 0.3f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, chillTimer);

            if (!isDead)
            {
                fx.ChillFXFor(chillTimer);
            }
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                {
                    return;
                }

                HitNearestTargetWithShockStrike();
            }
        }

    }

    public void ApplyShock(bool _Shock)
    {
        if (isShocked)
        {
            return;
        }

        isShocked = _Shock;
        shockTimer = ailmentsDuration;

        if (!isDead)
        {
            fx.ShockFXFor(shockTimer);
        }
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 18);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1f)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(float _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(float _damage) => shockDamage = _damage;

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0 && !isDead)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0)
            {
                Die();
            }

            ignitedDamageTimer = igniteDamageCooldown;
        }
    }

    #endregion

    public virtual void TakeDamage(float _damage)
    {
        if (isDead)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void DecreaseHealthBy(float _damage)
    {
        if (isVulnerable)
        {
            _damage = _damage * 1.1f;
        }

        currentHealth -= _damage;

        //伤害显示(如果有空的话就对不同的伤害显示不同的颜色吧)
        if ((int)_damage > 0)
        {
            fx.CreatePopUpText(((int)_damage).ToString());
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    public virtual void IncreaseHealthBy(float _amount)
    {
        currentHealth += _amount;

        if (isDead)
        {
            return;
        }

        if (currentHealth >= GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
        {
            DecreaseHealthBy(currentHealth);
            Die();
        }
    }

    /// <summary>
    /// 设置无敌状态
    /// </summary>
    /// <param name="_invencible">如果为True,则处于无敌状态,反之为正常状态</param>
    public void MakeInvencible(bool _invencible) => isInvincible = _invencible;

    #region Start calculations

    public virtual void OnEvasion()
    {
    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        float totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (UnityEngine.Random.Range(0, 100) <= totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 防御后的伤害 = 敌人的攻击 - 目标的防御（最低造成 1 点伤害）
    /// </summary>
    /// <param name="_targetStats">目标</param>
    /// <param name="_totalDamage">敌人伤害</param>
    /// <returns></returns>
    protected float CheckTargetArmor(CharacterStats _targetStats, float _totalDamage)
    {
        if (_targetStats.isChilled)
        {
            _totalDamage -= _targetStats.armor.GetValue() * 0.8f;
        }
        else
        {
            _totalDamage -= _targetStats.armor.GetValue();
        }

        _totalDamage = Mathf.Clamp(_totalDamage, 1, int.MaxValue);
        return _totalDamage;
    }

    /// <summary>
    /// 抵抗后的伤害 = 敌人的魔法总攻击 - 目标智力 * 3 - 目标魔法抵抗值
    /// </summary>
    /// <param name="_targetStats">目标</param>
    /// <param name="_totalMagicalDamage">我方总魔法伤害</param>
    /// <returns></returns>
    public float CheckTargetResistance(CharacterStats _targetStats, float _totalMagicalDamage)
    {
        _totalMagicalDamage -= _targetStats.intelligence.GetValue() * 3 + _targetStats.magicResistance.GetValue();
        _totalMagicalDamage = Mathf.Clamp(_totalMagicalDamage, 0, int.MaxValue);
        return _totalMagicalDamage;
    }

    protected bool CanCrit()
    {
        float totalCritChance = critateChance.GetValue() + agility.GetValue();

        if (UnityEngine.Random.Range(0, 100) <= totalCritChance)
        {
            return true;
        }

        return false;
    }

    protected float CalculateCritialDamage(float _damage)
    {
        float totalCritatePower = critatePower.GetValue() + strength.GetValue() * 0.01f;
        float critateDamage = _damage * totalCritatePower / 100;

        return (float)Math.Round(critateDamage, 2);
    }

    public float GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    #endregion

    /// <summary>
    /// 上buff种类
    /// </summary>
    /// <returns>返回种类</returns>
    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength:
                return strength;

            case StatType.agility:
                return agility;

            case StatType.intelligence:
                return intelligence;

            case StatType.vitality:
                return vitality;

            case StatType.damage:
                return damage;

            case StatType.critateChance:
                return critateChance;

            case StatType.critatePower:
                return critatePower;

            case StatType.health:
                return maxHealth;

            case StatType.armor:
                return armor;

            case StatType.evasion:
                return evasion;

            case StatType.magicResistance:
                return magicResistance;

            case StatType.fireDamage:
                return fireDamage;

            case StatType.iceDamage:
                return iceDamage;

            case StatType.lightningDamage:
                return lightningDamage;

            default:
                return null;
        }
    }
}
