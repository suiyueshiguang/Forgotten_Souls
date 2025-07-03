using Loxodon.Framework.ViewModels;
using UnityEngine;

public class UI_InGameViewModel : ViewModelBase
{
    private UI_InGameModel model = new UI_InGameModel();
    private ISkillManager skillManager;
    private IPlayerManager playerManager;

    #region ���Hp��ʾ
    public float hpValue
    {
        get => model.hpValue;
        set => this.Set<float>(ref model.hpValue, value, "hpValue");
    }

    public float hpMaxValue
    {
        get => model.hpMaxValue;
        set => this.Set<float>(ref model.hpMaxValue, value, "hpMaxValue");
    }

    public float hpEffectValue
    {
        get => model.hpEffectValue;
        set => this.Set<float>(ref model.hpEffectValue, value, "hpEffectValue");
    }

    public float hpEffectMaxValue
    {
        get => model.hpEffectMaxValue;
        set => this.Set<float>(ref model.hpEffectMaxValue, value, "hpEffectMaxValue");
    }

    public float hpEffectSpeed
    {
        get => model.hpEffectSpeed;
        set => this.Set<float>(ref model.hpEffectSpeed, value, "hpEffectSpeed");
    }
    #endregion

    #region ������ȴ״̬��0-1��ʾ���״̬��
    public float dashCooldown
    {
        get => model.dashCooldown;
        set => this.Set<float>(ref model.dashCooldown, value, "dashCooldown");
    }

    public float parryCooldown
    {
        get => model.parryCooldown;
        set => this.Set<float>(ref model.parryCooldown, value, "parryCooldown");
    }

    public float crystalCooldown
    {
        get => model.crystalCooldown;
        set => this.Set<float>(ref model.crystalCooldown, value, "crystalCooldown");
    }

    public float swordCooldown
    {
        get => model.swordCooldown;
        set => this.Set<float>(ref model.swordCooldown, value, "swordCooldown");
    }

    public float blackHoleCooldown
    {
        get => model.blackHoleCooldown;
        set => this.Set<float>(ref model.blackHoleCooldown, value, "blackHoleCooldown");
    }

    public float flaskCooldown
    {
        get => model.flaskCooldown;
        set => this.Set<float>(ref model.flaskCooldown, value, "flaskCooldown");
    }
    #endregion

    #region ������ʾ
    public int currentSouls
    {
        get => model.currentSouls;
        set => this.Set<int>(ref model.currentSouls, value, "currentSouls");
    }

    public float soulsIncreaseRate
    {
        get => model.increaseRate;
        set => this.Set<float>(ref model.increaseRate, value, "soulsIncreaseRate");
    }
    #endregion

    public void Start()
    {
        skillManager = ServiceLocator.GetService<ISkillManager>();
        playerManager = ServiceLocator.GetService<IPlayerManager>();
    }

    public void Update(float _daltaTime)
    {
        UpdateSouls(_daltaTime);
        UpdateCooldown(_daltaTime);
        UpdateHpEffect(_daltaTime);
    }

    /// <summary>
    /// ����������Ӿ��ϵ�����
    /// </summary>
    private void UpdateSouls(float _daltaTime)
    {
        if (currentSouls < playerManager.currency)
        {
            currentSouls += Mathf.FloorToInt(_daltaTime * soulsIncreaseRate);
        }
        else
        {
            currentSouls = playerManager.currency;
        }
    }

    /// <summary>
    /// ������ȴ
    /// </summary>
    /// <param name="_daltaTime"></param>
    private void UpdateCooldown(float _daltaTime)
    {
        if (dashCooldown > 0)
        {
            dashCooldown -= _daltaTime / skillManager.GetDash().cooldown;
        }
        if (parryCooldown > 0)
        {
            parryCooldown -= _daltaTime / skillManager.GetParry().cooldown;
        }
        if (crystalCooldown > 0)
        {
            crystalCooldown -= _daltaTime / skillManager.GetCrystal().cooldown;
        }
        if (swordCooldown > 0)
        {
            swordCooldown -= _daltaTime / skillManager.GetSword().cooldown;
        }
        if (blackHoleCooldown > 0)
        {
            blackHoleCooldown -= _daltaTime / skillManager.GetBlackhole().cooldown;
        }
        if (flaskCooldown > 0)
        {
            flaskCooldown -= _daltaTime / ServiceLocator.GetService<IInventory>().GetFlaskCooldown();
        }
    }

    private void UpdateHpEffect(float _deltaTime)
    {
        if (hpEffectValue > hpValue)
        {
            hpEffectValue -= hpEffectSpeed * _deltaTime;
        }
        else
        {
            hpEffectValue = hpValue;
        }
    }

    /// <summary>
    /// ����������ȴʱ��
    /// </summary>
    /// <param name="skillType">��������</param>
    public void TriggerSkillCooldown(SkillInGameType skillType)
    {
        switch (skillType)
        {
            case SkillInGameType.Dash:
                {
                    if (skillManager.GetDash().dashUnlocked && dashCooldown <= 0)
                    {
                        dashCooldown = 1;
                    }

                    break;
                }
            case SkillInGameType.Parry:
                {
                    if (skillManager.GetParry().parryUnlocked && parryCooldown <= 0)
                    {
                        parryCooldown = 1;
                    }

                    break;
                }
            case SkillInGameType.Crystal:
                {
                    if (skillManager.GetCrystal().crystalUnlocked && crystalCooldown <= 0)
                    {
                        crystalCooldown = 1;
                    }

                    break;
                }
            case SkillInGameType.Sword:
                {
                    if (skillManager.GetSword().swordUnlocked && swordCooldown <= 0)
                    {
                        swordCooldown = 1;
                    }

                    break;
                }
            case SkillInGameType.BlackHole:
                {
                    if (skillManager.GetBlackhole().blackHoleUnlocked && blackHoleCooldown <= 0 && playerManager.GetPlayer().playerStateFactory.playerState is PlayerBlackholeState)
                    {
                        blackHoleCooldown = 1;
                    }
                    break;
                }
            case SkillInGameType.Flask:
                {
                    if (ServiceLocator.GetService<IInventory>().GetEquipment(EquipmentType.Flask) != null && flaskCooldown <= 0)
                    {
                        flaskCooldown = 1;
                    }

                    break;
                }
        }
    }
}
