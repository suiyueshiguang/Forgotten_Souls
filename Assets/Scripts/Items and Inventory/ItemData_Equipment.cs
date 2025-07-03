using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "数据/装备")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("特效")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;

    [Header("主要数值")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("进攻数值")]
    public int damage;        
    public int critateChance;
    public int critatePower;

    [Header("防御数值")]
    public int health;    
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("魔法信息")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("制作要求")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    private PlayerStats playerStats => ServiceLocator.GetService<IPlayerManager>().GetPlayer().GetComponent<PlayerStats>();

    /// <summary>
    /// 装备执行的效果显示的位置
    /// </summary>
    /// <param name="_enemyPosition">角色的位置</param>
    public void Effect(Transform _enemyPosition)
    {
        foreach (ItemEffect itemEffect in itemEffects)
        {
            itemEffect.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        playerStats.strength.AddModifier(strength); 
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critateChance.AddModifier(critateChance);
        playerStats.critatePower.AddModifier(critatePower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);

        if (health > 0)
        {
            playerStats.UpdateUIHealthBy(health);
        }
    }

    public void RemoveModifiers()
    {
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critateChance.RemoveModifier(critateChance);
        playerStats.critatePower.RemoveModifier(critatePower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);

        if (health > 0)
        {
            playerStats.UpdateUIHealthBy(health);
        }
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力");
        AddItemDescription(vitality, "血量");

        AddItemDescription(damage, "攻击力");
        AddItemDescription(critateChance, "暴击率");
        AddItemDescription(critatePower, "暴击伤害");

        AddItemDescription(health, "生命");
        AddItemDescription(armor, "防御力");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(magicResistance, "魔法抗性");

        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "冰冻伤害");
        AddItemDescription(lightningDamage, "雷电伤害");

        if(descriptionLength < 5 && !equipmentType.ToString().Equals("Flask"))
        {
            for (int index = 0; index < 4 - descriptionLength; index++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        for (int index = 0; index < itemEffects.Length; index++)
        {
            if (itemEffects[index].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.Append("效果: " + itemEffects[index].effectDescription);
                descriptionLength++;
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if(_value != 0)
        {
            if(sb.Length > 0)
            {
                sb.AppendLine();
            }

            if (_value > 0)
            {
                sb.Append("+ " + _value + (_value.ToString().Length == 1 ? "  " : " ") + _name);
            }
        descriptionLength ++;
        }
    }
}
