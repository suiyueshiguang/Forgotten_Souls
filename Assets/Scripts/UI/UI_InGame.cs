using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : UIView
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider hpEffectSlider;
    [SerializeField] private float hpEffectSpeed;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image flaskImage;

    [Header("����ȡ")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float increaseRate = 250;

    private UI_InGameViewModel inGameViewModel;

    protected override void Awake()
    {
        //����Ӧ��������
        ApplicationContext context = Context.GetApplicationContext();

        //��ʼ�����ݰ󶨷���
        BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
        bindingService.Start();
    }

    protected override void Start()
    {
        playerStats = ServiceLocator.GetService<IPlayerManager>().GetPlayerGameObject().GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.onHealthChanged += HandleHealthChanged;
        }

        //��������ʼ��ViewModel
        inGameViewModel = new UI_InGameViewModel()
        {
            //��UI���ֱ�Ӹ�ֵ��ViewModel
            hpValue = hpSlider.value,
            hpMaxValue = hpSlider.maxValue,
            hpEffectValue = hpEffectSlider.value,
            hpEffectMaxValue = hpEffectSlider.maxValue,
            hpEffectSpeed = this.hpEffectSpeed,

            dashCooldown = dashImage.fillAmount,
            parryCooldown = parryImage.fillAmount,
            crystalCooldown = crystalImage.fillAmount,
            swordCooldown = swordImage.fillAmount,
            blackHoleCooldown = blackHoleImage.fillAmount,
            flaskCooldown = flaskImage.fillAmount,

            currentSouls = int.TryParse(currentSouls.text, out int value) ? value : 0,
            soulsIncreaseRate = increaseRate
        };

        inGameViewModel.Start();

        //��������������
        this.SetDataContext(inGameViewModel);

        SetUpBindings();

        HandleHealthChanged();
    }

    private void Update()
    {
        inGameViewModel.Update(Time.deltaTime);

        if (Input.GetButtonDown("Skill_Dash"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.Dash);
        }
        if (Input.GetButtonDown("Skill_Parry"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.Parry);
        }
        if (Input.GetButtonDown("Skill_Crystal"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.Crystal);
        }
        if (Input.GetButtonDown("Skill_Sword"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.Sword);
        }
        if (Input.GetButtonDown("Skill_BlackHole"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.BlackHole);
        }
        if (Input.GetButtonDown("Flask"))
        {
            inGameViewModel.TriggerSkillCooldown(SkillInGameType.Flask);
        }
    }

    private void SetUpBindings()
    {
        //�������ݰ�
        BindingSet<UI_InGame, UI_InGameViewModel> bindingSet = this.CreateBindingSet<UI_InGame, UI_InGameViewModel>();

        //Ѫ����
        bindingSet.Bind(hpSlider)
            .For(view => view.value)
            .To(viewmodel => viewmodel.hpValue)
            .OneWay();
        bindingSet.Bind(hpSlider)
            .For(view => view.maxValue)
            .To(viewmodel => viewmodel.hpMaxValue)
            .OneWay();
        bindingSet.Bind(hpEffectSlider)
            .For(view => view.value)
            .To(viewmodel => viewmodel.hpEffectValue)
            .OneWay();
        bindingSet.Bind(hpEffectSlider)
            .For(view => view.maxValue)
            .To(viewmodel => viewmodel.hpEffectMaxValue)
            .OneWay();

        //������ȴ��
        bindingSet.Bind(dashImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.dashCooldown)
            .OneWay();
        bindingSet.Bind(parryImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.parryCooldown)
            .OneWay();
        bindingSet.Bind(crystalImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.crystalCooldown)
            .OneWay();
        bindingSet.Bind(swordImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.swordCooldown)
            .OneWay();
        bindingSet.Bind(blackHoleImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.blackHoleCooldown)
            .OneWay();
        bindingSet.Bind(flaskImage)
            .For(view => view.fillAmount)
            .To(viewmodel => viewmodel.flaskCooldown)
            .OneWay();

        //��Ǯ��
        bindingSet.Bind(currentSouls)
            .For(view => view.text)
            .ToExpression(viewmodel => viewmodel.currentSouls.ToString())
            .OneWay();

        bindingSet.Build();
    }

    private void HandleHealthChanged()
    {
        inGameViewModel.hpMaxValue = playerStats.GetMaxHealthValue();
        inGameViewModel.hpValue = playerStats.currentHealth;

        inGameViewModel.hpEffectMaxValue = playerStats.GetMaxHealthValue();
    }
}
