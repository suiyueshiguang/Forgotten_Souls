using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterStats, ISaveManager
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();

        player.Die();

        ServiceLocator.GetService<IGameManager>().lostCurrencyAmount = ServiceLocator.GetService<IPlayerManager>().currency;
        ServiceLocator.GetService<IPlayerManager>().currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(float _damage)
    {
        base.DecreaseHealthBy(_damage);

        //受到致命伤害
        if (_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6));
            player.fx.ScreenShake(player.fx.shakeHighDamage);

            /*
             * int randomSound = Random.Range(9, 15);
             * ServiceLocator.GetService<IAudioManager>().PlaySFX(randomSound, null);
            */
        }

        ItemData_Equipment currentArmor = ServiceLocator.GetService<IInventory>()?.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public override void OnEvasion()
    {
        player.skill.GetDodge().CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        float totalDamage = damage.GetValue() + strength.GetValue();

        ServiceLocator.GetService<IAudioManager>().PlaySFX("SwordSwing", null);

        if (_multiplier > 0)
        {
            totalDamage = totalDamage * _multiplier;
        }

        if (CanCrit())
        {
            totalDamage = CalculateCritialDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);

        //Debug.Log("分身造成：" + totalDamage);
    }

    /// <summary>
    /// 装备了增加血量的装备后,只对最大血量进行更新
    /// </summary>
    /// <param name="_amount">血量值</param>
    public void UpdateUIHealthBy(float _amount) => StartCoroutine(UpdateUIHealthIEnumerator());

    private IEnumerator UpdateUIHealthIEnumerator()
    {
        yield return null;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }

        if (currentHealth > GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }
    }

    public void LoadData(GameData _data)
    {
        currentHealth = _data.health;
    }

    public void SaveData(ref GameData _data)
    {
        _data.health = currentHealth;
    }
}
