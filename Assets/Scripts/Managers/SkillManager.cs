using UnityEngine;

public class SkillManager : MonoBehaviour, ISkillManager
{
    private Dash_Skill dash;
    private Clone_Skill clone;
    private Sword_Skill sword;
    private Blackhole_Skill blackhole;
    private Crystal_Skill crystal;
    private Parry_Skill parry;
    private Dodge_Skill dodge;

    #region GetValues
    public Dash_Skill GetDash() => dash;

    public Clone_Skill GetClone() => clone;

    public Sword_Skill GetSword() => sword;

    public Blackhole_Skill GetBlackhole() => blackhole;

    public Crystal_Skill GetCrystal() => crystal;

    public Parry_Skill GetParry() => parry;

    public Dodge_Skill GetDodge() => dodge;
    #endregion

    private void Awake()
    {
        if (ServiceLocator.GetService<ISkillManager>() == null)
        {
            ServiceLocator.Register<ISkillManager>(this);
        }
    }

    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
        parry = GetComponent<Parry_Skill>();
        dodge = GetComponent<Dodge_Skill>();
    }
}
