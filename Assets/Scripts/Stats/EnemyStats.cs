using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;

    [Header("µ»º∂…À∫¶")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = 0.05f;

    protected override void Start()
    {
        base.Start();
        soulsDropAmount.SetDefalutValut(100);

        ApplyLevelModifiers();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critateChance);
        Modify(critatePower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);

        Modify(soulsDropAmount);
    }

    private void Modify(Stat _stat)
    {
        for (int index = 1; index < level; index++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier((float)Math.Round(modifier, 2));
        }
    }

    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();

        enemy.Die();
        enemy.CanBeStunned();

        ServiceLocator.GetService<IPlayerManager>().currency += ((int)soulsDropAmount.GetValue());
        myDropSystem.GenerateDrop();

        Destroy(gameObject, 5f);
    }
}
